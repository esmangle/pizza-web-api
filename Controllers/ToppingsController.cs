using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaManagerAPI.Data;
using PizzaManagerAPI.Models;

namespace PizzaManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToppingsController : ControllerBase
{
	private readonly PizzaDbContext _context;

	public ToppingsController(PizzaDbContext context)
	{
		_context = context;
	}

	// GET api/Toppings
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Topping>>> GetToppings()
	{
		return await _context.Toppings.ToListAsync();
	}

	// GET api/Toppings/5
	[HttpGet("{id}")]
	public async Task<ActionResult<Topping>> GetTopping(int id)
	{
		var topping = await _context.Toppings.FindAsync(id);

		if (topping == null)
		{
			return NotFound();
		}

		return topping;
	}

	// POST api/Toppings
	[HttpPost]
	public async Task<ActionResult<Topping>> PostTopping(ToppingPostDto toppingDto)
	{
		bool isDupe = await _context.Toppings.AnyAsync(
			t => t.Name.Equals(toppingDto.Name, StringComparison.OrdinalIgnoreCase)
		);

		if (isDupe)
		{
			return Conflict($"A topping with the name '{toppingDto.Name}' already exists.");
		}

		var topping = new Topping { Name = toppingDto.Name };

		// autoincrement
		topping.Id = (await _context.Toppings.MaxAsync(t => (int?) t.Id) ?? 0) + 1;

		_context.Toppings.Add(topping);

		await _context.SaveChangesAsync();

		return CreatedAtAction("GetTopping", new { id = topping.Id }, topping);
	}

	// PUT api/Toppings/5
	[HttpPut("{id}")]
	public async Task<IActionResult> PutTopping(int id, ToppingPutDto toppingDto)
	{
		var topping = await _context.Toppings.FindAsync(id);

		if (topping == null)
		{
			return NotFound();
		}

		bool isDupe = await _context.Toppings.AnyAsync(
			t => t.Id != id && t.Name.Equals(toppingDto.Name, StringComparison.OrdinalIgnoreCase)
		);

		if (isDupe)
		{
			return Conflict($"A topping with the name '{toppingDto.Name}' already exists.");
		}

		topping.Name = toppingDto.Name;

		_context.Entry(topping).State = EntityState.Modified;

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			if (!_context.Toppings.Any(e => e.Id == id))
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

	// DELETE api/Toppings/5
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteTopping(int id)
	{
		var topping = await _context.Toppings.FindAsync(id);

		if (topping == null)
		{
			return NotFound();
		}

		bool isInUse = await _context.PizzaToppings.AnyAsync(pt => pt.ToppingId == id);
		if (isInUse)
		{
			return Conflict("Cannot delete topping because it is used on one or more pizzas.");
		}

		_context.Toppings.Remove(topping);

		await _context.SaveChangesAsync();

		return NoContent();
	}
}
