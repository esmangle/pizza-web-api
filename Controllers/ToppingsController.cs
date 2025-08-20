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
		return NoContent();
	}

	// GET api/Toppings/5
	[HttpGet("{id}")]
	public async Task<ActionResult<Topping>> GetTopping(int id)
	{
		return NoContent();
	}

	// POST api/Toppings
	[HttpPost]
	public async Task<ActionResult<Topping>> PostTopping(Topping topping)
	{
		return NoContent();
	}

	// PUT api/Toppings/5
	[HttpPut("{id}")]
	public async Task<IActionResult> PutTopping(int id, Topping topping)
	{
		return NoContent();
	}

	// DELETE api/Toppings/5
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteTopping(int id)
	{
		return NoContent();
	}
}
