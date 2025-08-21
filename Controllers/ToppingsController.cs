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

	// GET api/Toppings/1
	[HttpGet("{id}")]
	public async Task<ActionResult<ToppingResponse>> GetTopping(int id)
	{
		var result = await _service.GetToppingByIdAsync(id);

		return result switch
		{
			OkResult<ToppingResponse> okResult => Ok(okResult.Value),

			NotFoundResult<ToppingResponse> => NotFound(),

			_ => BadRequest()
		};
	}

	// POST api/Toppings
	[HttpPost]
	public async Task<ActionResult<ToppingResponse>> PostTopping(ToppingCreateDto toppingDto)
	{
		var result = await _service.CreateToppingAsync(toppingDto);

		return result switch
		{
			OkResult<ToppingResponse> { Value: var topping } => CreatedAtAction(
				nameof(GetTopping), new { id = topping.Id }, topping
			),

			NotFoundResult<ToppingResponse> => NotFound(),

			DuplicateResult<ToppingResponse> dupeResult => Conflict(new
			{
				Message = $"A topping with the name '{toppingDto.Name}' already exists.",
				ErrorCode = "DUPLICATE_NAME",
				ConflictingResource = dupeResult.ConflictValue,
			}),

			_ => BadRequest()
		};
	}

	// PUT api/Toppings/1
	[HttpPut("{id}")]
	public async Task<ActionResult<ToppingResponse>> PutTopping(int id, ToppingUpdateDto toppingDto)
	{
		var result = await _service.UpdateToppingAsync(id, toppingDto);

		return result switch
		{
			OkResult<ToppingResponse> { Value: var topping } => CreatedAtAction(
				nameof(GetTopping), new { id = topping.Id }, topping
			),

			NotFoundResult<ToppingResponse> => NotFound(),

			DuplicateResult<ToppingResponse> dupeResult => Conflict(new
			{
				Message = $"A topping with the name '{toppingDto.Name}' already exists.",
				ErrorCode = "DUPLICATE_NAME",
				ConflictingResource = dupeResult.ConflictValue,
			}),

			_ => BadRequest()
		};
	}

	// DELETE api/Toppings/1
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteTopping(int id)
	{
		var result = await _service.DeleteToppingAsync(id);

		return result switch
		{
			{ IsSuccess: true } => NoContent(),

			NotFoundResult<ToppingResponse> => NotFound(),

			ToppingInUseResult inUseResult => Conflict(new
			{
				Message = "Cannot delete topping because it is used by one or more pizzas.",
				ErrorCode = "TOPPING_IN_USE",
				PizzaIds = inUseResult.PizzaIds,
			}),

			_ => BadRequest()
		};
	}
}
