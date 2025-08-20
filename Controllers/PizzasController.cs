using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaManagerAPI.Data;
using PizzaManagerAPI.Models;

namespace PizzaManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PizzasController : ControllerBase
{
	private readonly PizzaDbContext _context;

	public PizzasController(PizzaDbContext context)
	{
		_context = context;
	}

	// GET api/Pizzas
	[HttpGet]
	public async Task<ActionResult<IEnumerable<PizzaResponse>>> GetPizzas()
	{
		var pizzas = await _context.Pizzas
			.Include(p => p.PizzaToppings)
			.ThenInclude(pt => pt.Topping)
			.ToListAsync();

		return pizzas.Select(p => MapToResponse(p)).ToList();
	}

	// GET api/Pizzas/5
	[HttpGet("{id}")]
	public async Task<ActionResult<PizzaResponse>> GetPizza(int id)
	{
		var pizza = await _context.Pizzas
			.Include(p => p.PizzaToppings)
			.ThenInclude(pt => pt.Topping)
			.FirstOrDefaultAsync(p => p.Id == id);

		if (pizza == null)
		{
			return NotFound();
		}

		return MapToResponse(pizza);
	}

	// POST api/Pizzas
	[HttpPost]
	public async Task<ActionResult<PizzaResponse>> PostPizza(PizzaPostDto pizzaDto)
	{
		var (isDupe, invalidToppingIds) = await ValidatePizzaAsync(pizzaDto.Name, pizzaDto.ToppingIds);

		if (isDupe)
		{
			return Conflict($"A pizza with the name '{pizzaDto.Name}' already exists.");
		}

		if (invalidToppingIds.Any())
		{
			return BadRequest(new
			{
				Message = $"Invalid topping IDs: {string.Join(", ", invalidToppingIds)}",
				ToppingIds = invalidToppingIds
			});
		}

		using var transaction = await _context.Database.BeginTransactionAsync();

		try
		{
			var pizza = new Pizza { Name = pizzaDto.Name };

			// autoincrement (this is only necessary due to the in-memory database)
			pizza.Id = (await _context.Pizzas.MaxAsync(p => (int?)p.Id) ?? 0) + 1;

			_context.Pizzas.Add(pizza);

			_context.PizzaToppings.AddRange(
				pizzaDto.ToppingIds.Select(toppingId =>
					new PizzaTopping { PizzaId = pizza.Id, ToppingId = toppingId }
				)
			);

			await _context.SaveChangesAsync();
			await transaction.CommitAsync();

			return CreatedAtAction(nameof(GetPizza), new { id = pizza.Id }, MapToResponse(pizza));
		}
		catch
		{
			await transaction.RollbackAsync();
			throw;
		}
	}

	// PUT api/Pizzas/5
	[HttpPut("{id}")]
	public async Task<IActionResult> PutPizza(int id, PizzaPutDto pizzaDto)
	{
		var pizza = await _context.Pizzas
			.Include(p => p.PizzaToppings)
			.FirstOrDefaultAsync(p => p.Id == id);

		if (pizza == null)
		{
			return NotFound();
		}

		var (isDupe, invalidToppingIds) = await ValidatePizzaAsync(pizzaDto.Name, pizzaDto.ToppingIds);

		if (isDupe)
		{
			return Conflict($"A pizza with the name '{pizzaDto.Name}' already exists.");
		}

		if (invalidToppingIds.Any())
		{
			return BadRequest(new
			{
				Message = $"Invalid topping IDs: {string.Join(", ", invalidToppingIds)}",
				ToppingIds = invalidToppingIds
			});
		}

		using var transaction = await _context.Database.BeginTransactionAsync();

		try
		{
			pizza.Name = pizzaDto.Name;

			var existingToppingIds = pizza.PizzaToppings
				.Select(pt => pt.ToppingId)
				.ToList();

			var toppingsToRemove = pizza.PizzaToppings
				.Where(pt => !pizzaDto.ToppingIds.Contains(pt.ToppingId))
				.ToList();

			var toppingsToAdd = pizzaDto.ToppingIds
				.Except(existingToppingIds)
				.Select(toppingId =>
					new PizzaTopping { PizzaId = pizza.Id, ToppingId = toppingId }
				);

			_context.PizzaToppings.RemoveRange(toppingsToRemove);
			_context.PizzaToppings.AddRange(toppingsToAdd);

			await _context.SaveChangesAsync();
			await transaction.CommitAsync();

			return NoContent();
		}
		catch (DbUpdateConcurrencyException)
		{
			await transaction.RollbackAsync();

			if (!_context.Pizzas.Any(e => e.Id == id))
			{
				return NotFound();
			}
			else
			{
				throw;
			}
		}
		catch
		{
			await transaction.RollbackAsync();
			throw;
		}
	}

	// DELETE api/Pizzas/5
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeletePizza(int id)
	{
		var pizza = await _context.Pizzas
			.Include(p => p.PizzaToppings)
			.FirstOrDefaultAsync(p => p.Id == id);

		if (pizza == null)
		{
			return NotFound();
		}

		using var transaction = await _context.Database.BeginTransactionAsync();

		try
		{
			_context.PizzaToppings.RemoveRange(pizza.PizzaToppings);
			_context.Pizzas.Remove(pizza);

			await _context.SaveChangesAsync();
			await transaction.CommitAsync();

			return NoContent();
		}
		catch
		{
			await transaction.RollbackAsync();
			throw;
		}
	}

	private async Task<(bool isDupe, List<int> invalidToppingIds)> ValidatePizzaAsync(
		string name, List<int> toppingIds, int? id = null)
	{
		var dupeTask = _context.Pizzas.AnyAsync(p =>
			(id == null || p.Id != id) && p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
		);

		var toppingsTask = _context.Toppings
			.Where(t => toppingIds.Contains(t.Id))
			.Select(t => t.Id)
			.ToListAsync();

		await Task.WhenAll(dupeTask, toppingsTask);

		return (dupeTask.Result, toppingIds.Except(toppingsTask.Result).ToList());
	}

	private PizzaResponse MapToResponse(Pizza pizza)
	{
		return new PizzaResponse
		{
			Id = pizza.Id,
			Name = pizza.Name,
			Toppings = pizza.PizzaToppings.Select(
				pt => new PizzaToppingResponse {
					Id = pt.Topping.Id,
					Name = pt.Topping.Name
				}
			).ToList()
		};
	}
}
