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
		bool isDupe = await _context.Pizzas.AnyAsync(
			p => p.Name.Equals(pizzaDto.Name, StringComparison.OrdinalIgnoreCase)
		);

		if (isDupe)
		{
			return Conflict($"A pizza with the name '{pizzaDto.Name}' already exists.");
		}

		var invalidToppingIds = pizzaDto.ToppingIds
			.Except(await _context.Toppings.Select(t => t.Id).ToListAsync())
			.ToList();

		if (invalidToppingIds.Any())
		{
			return BadRequest($"Invalid topping IDs: {string.Join(", ", invalidToppingIds)}");
		}

		var pizza = new Pizza { Name = pizzaDto.Name };

		// autoincrement (this is only necessary due to the in-memory database)
		pizza.Id = (await _context.Pizzas.MaxAsync(p => (int?)p.Id) ?? 0) + 1;

		_context.Pizzas.Add(pizza);

		foreach (var toppingId in pizzaDto.ToppingIds)
		{
			_context.PizzaToppings.Add(new PizzaTopping {
				PizzaId = pizza.Id, ToppingId = toppingId
			});
		}

		await _context.SaveChangesAsync();

		var newPizza = await _context.Pizzas
			.Include(p => p.PizzaToppings)
			.ThenInclude(pt => pt.Topping)
			.FirstOrDefaultAsync(p => p.Id == pizza.Id);

		if (newPizza == null)
		{
			return NotFound();
		}

		return CreatedAtAction("GetPizza", new { id = pizza.Id }, MapToResponse(newPizza));
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

		if (!pizza.Name.Equals(pizzaDto.Name, StringComparison.OrdinalIgnoreCase))
		{
			bool isDupe = await _context.Pizzas.AnyAsync(
				p => p.Id != id && p.Name.Equals(pizzaDto.Name, StringComparison.OrdinalIgnoreCase)
			);

			if (isDupe)
			{
				return Conflict($"A pizza with the name '{pizzaDto.Name}' already exists.");
			}
		}

		var invalidToppingIds = pizzaDto.ToppingIds
			.Except(await _context.Toppings.Select(t => t.Id).ToListAsync())
			.ToList();

		if (invalidToppingIds.Any())
		{
			return BadRequest($"Invalid topping IDs: {string.Join(", ", invalidToppingIds)}");
		}

		pizza.Name = pizzaDto.Name;

		var existingToppingIds = pizza.PizzaToppings.Select(pt => pt.ToppingId).ToList();
		var toppingsToRemove = pizza.PizzaToppings.Where(pt => !pizzaDto.ToppingIds.Contains(pt.ToppingId)).ToList();
		var toppingsToAdd = pizzaDto.ToppingIds.Except(existingToppingIds).ToList();

		_context.PizzaToppings.RemoveRange(toppingsToRemove);

		foreach (var toppingId in toppingsToAdd)
		{
			_context.PizzaToppings.Add(new PizzaTopping{
				PizzaId = pizza.Id, ToppingId = toppingId
			});
		}

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			if (!_context.Pizzas.Any(e => e.Id == id))
			{
				return NotFound();
			}
			else
			{
				throw;
			}
		}

		return NoContent();
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

		_context.PizzaToppings.RemoveRange(pizza.PizzaToppings);
		_context.Pizzas.Remove(pizza);

		await _context.SaveChangesAsync();

		return NoContent();
	}

	private PizzaResponse MapToResponse(Pizza pizza)
	{
		return new PizzaResponse
		{
			Id = pizza.Id,
			Name = pizza.Name,
			Toppings = pizza.PizzaToppings
				.Select(pt => new PizzaToppingResponse {
					Id = pt.Topping.Id, Name = pt.Topping.Name
				}).ToList()
		};
	}
}
