using Microsoft.AspNetCore.Mvc;
using PizzaManagerAPI.Models;
using PizzaManagerAPI.Services;
using PizzaManagerAPI.Services.Results;

namespace PizzaManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToppingsController : ControllerBase
{
	private readonly IToppingService _service;

	public ToppingsController(IToppingService service)
	{
		_service = service;
	}

	// GET api/Toppings
	[HttpGet]
	public async Task<ActionResult<IEnumerable<ToppingResponse>>> GetToppings()
	{
		return Ok(await _service.GetAllToppingsAsync());
	}

	// GET api/Toppings/5
	[HttpGet("{id}")]
	public async Task<ActionResult<ToppingResponse>> GetTopping(int id)
	{
		var result = await _service.GetToppingByIdAsync(id);

		if (result is OkResult<ToppingResponse> okResult)
		{
			return Ok(okResult.Value);
		}

		if (result is NotFoundResult<ToppingResponse>)
		{
			return NotFound();
		}

		return BadRequest();
	}

	// POST api/Toppings
	[HttpPost]
	public async Task<ActionResult<ToppingResponse>> PostTopping(ToppingCreateDto toppingDto)
	{
		var result = await _service.CreateToppingAsync(toppingDto);

		if (result is OkResult<ToppingResponse> okResult)
		{
			var topping = okResult.Value;

			return CreatedAtAction(nameof(GetTopping), new { id = topping.Id }, topping);
		}

		if (result is NotFoundResult<ToppingResponse>)
		{
			return NotFound();
		}

		if (result is DuplicateResult<ToppingResponse> dupeResult)
		{
			return Conflict(new
			{
				Message = $"A topping with the name '{toppingDto.Name}' already exists.",
				ErrorCode = "DUPLICATE_NAME",
				ConflictingResource = dupeResult.ConflictValue,
			});
		}

		return BadRequest();
	}

	// PUT api/Toppings/5
	[HttpPut("{id}")]
	public async Task<ActionResult<ToppingResponse>> PutTopping(int id, ToppingUpdateDto toppingDto)
	{
		var result = await _service.UpdateToppingAsync(id, toppingDto);

		if (result is OkResult<ToppingResponse> okResult)
		{
			var topping = okResult.Value;

			return CreatedAtAction(nameof(GetTopping), new { id = topping.Id }, topping);
		}

		if (result is NotFoundResult<ToppingResponse>)
		{
			return NotFound();
		}

		if (result is DuplicateResult<ToppingResponse> dupeResult)
		{
			return Conflict(new
			{
				Message = $"A topping with the name '{toppingDto.Name}' already exists.",
				ErrorCode = "DUPLICATE_NAME",
				ConflictingResource = dupeResult.ConflictValue,
			});
		}

		return BadRequest();
	}

	// DELETE api/Toppings/5
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteTopping(int id)
	{
		var result = await _service.DeleteToppingAsync(id);

		if (result.IsSuccess)
		{
			return NoContent();
		}

		if (result is NotFoundResult<ToppingResponse>)
		{
			return NotFound();
		}

		if (result is ToppingInUseResult inUseResult)
		{
			return Conflict(new
			{
				Message = "Cannot delete topping because it is used by one or more pizzas.",
				ErrorCode = "TOPPING_IN_USE",
				PizzaIds = inUseResult.PizzaIds,
			});
		}

		return BadRequest();
	}
}
