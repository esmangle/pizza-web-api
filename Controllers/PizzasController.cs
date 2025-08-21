using Microsoft.AspNetCore.Mvc;
using PizzaManagerAPI.Models;
using PizzaManagerAPI.Services;
using PizzaManagerAPI.Services.Results;

namespace PizzaManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PizzasController : ControllerBase
{
	private readonly IPizzaService _service;

	public PizzasController(IPizzaService service)
	{
		_service = service;
	}

	// GET api/Pizzas
	[HttpGet]
	public async Task<ActionResult<IEnumerable<PizzaResponse>>> GetPizzas()
	{
		return Ok(await _service.GetAllPizzasAsync());
	}

	// GET api/Pizzas/5
	[HttpGet("{id}")]
	public async Task<ActionResult<PizzaResponse>> GetPizza(int id)
	{
		var result = await _service.GetPizzaByIdAsync(id);

		if (result is OkResult<PizzaResponse> okResult)
		{
			return Ok(okResult.Value);
		}

		if (result is NotFoundResult<PizzaResponse>)
		{
			return NotFound();
		}

		return BadRequest();
	}

	// POST api/Pizzas
	[HttpPost]
	public async Task<ActionResult<PizzaResponse>> PostPizza(PizzaCreateDto pizzaDto)
	{
		var result = await _service.CreatePizzaAsync(pizzaDto);

		if (result is OkResult<PizzaResponse> okResult)
		{
			var pizza = okResult.Value;

			return CreatedAtAction(nameof(GetPizza), new { id = pizza.Id }, pizza);
		}

		if (result is NotFoundResult<PizzaResponse>)
		{
			return NotFound();
		}

		if (result is DuplicateResult<PizzaResponse> dupeResult)
		{
			return Conflict(new
			{
				Message = $"A pizza with the name '{pizzaDto.Name}' already exists.",
				ErrorCode = "DUPLICATE_NAME",
				ConflictingResource = dupeResult.ConflictValue,
			});
		}

		if (result is InvalidToppingsResult badResult)
		{
			return BadRequest(new
			{
				Message = $"Invalid topping IDs: {string.Join(", ", badResult.InvalidToppingIds)}",
				ErrorCode = "INVALID_TOPPING_IDS",
				ToppingIds = badResult.InvalidToppingIds
			});
		}

		return BadRequest();
	}

	// PUT api/Pizzas/5
	[HttpPut("{id}")]
	public async Task<IActionResult> PutPizza(int id, PizzaUpdateDto pizzaDto)
	{
		var result = await _service.UpdatePizzaAsync(id, pizzaDto);

		if (result is OkResult<PizzaResponse> okResult)
		{
			var pizza = okResult.Value;

			return CreatedAtAction(nameof(GetPizza), new { id = pizza.Id }, pizza);
		}

		if (result is NotFoundResult<PizzaResponse>)
		{
			return NotFound();
		}

		if (result is DuplicateResult<PizzaResponse> dupeResult)
		{
			return Conflict(new
			{
				Message = $"A pizza with the name '{pizzaDto.Name}' already exists.",
				ErrorCode = "DUPLICATE_NAME",
				ConflictingResource = dupeResult.ConflictValue,
			});
		}

		if (result is InvalidToppingsResult badResult)
		{
			return BadRequest(new
			{
				Message = $"Invalid topping IDs: {string.Join(", ", badResult.InvalidToppingIds)}",
				ErrorCode = "INVALID_TOPPING_IDS",
				ToppingIds = badResult.InvalidToppingIds
			});
		}

		return BadRequest();
	}

	// DELETE api/Pizzas/5
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeletePizza(int id)
	{
		var result = await _service.DeletePizzaAsync(id);

		if (result.IsSuccess)
		{
			return NoContent();
		}

		if (result is NotFoundResult<PizzaResponse>)
		{
			return NotFound();
		}

		return BadRequest();
	}
}
