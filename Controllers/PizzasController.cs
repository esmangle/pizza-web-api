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
	public async Task<ActionResult<IEnumerable<Pizza>>> GetPizzas()
	{
		return NoContent();
	}

	// GET api/Pizzas/5
	[HttpGet("{id}")]
	public async Task<ActionResult<Pizza>> GetPizza(int id)
	{
		return NoContent();
	}

	// POST api/Pizzas
	[HttpPost]
	public async Task<ActionResult<Pizza>> PostPizza(PizzaPostDto pizzaDto)
	{
		return NoContent();
	}

	// PUT api/Pizzas/5
	[HttpPut("{id}")]
	public async Task<IActionResult> PutPizza(int id, PizzaPutDto pizzaDto)
	{
		return NoContent();
	}

	// DELETE api/Pizzas/5
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeletePizza(int id)
	{
		return NoContent();
	}
}

public class PizzaPostDto
{
	public required string Name { get; set; }

	public List<int> ToppingIds { get; set; } = new();
}

public class PizzaPutDto
{
	public required string Name { get; set; }

	public List<int> ToppingIds { get; set; } = new();
}
