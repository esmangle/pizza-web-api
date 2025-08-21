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

	// GET api/Pizzas/1
	[HttpGet("{id}")]
	public async Task<ActionResult<PizzaResponse>> GetPizza(int id)
	{
		var result = await _service.GetPizzaByIdAsync(id);

		return result switch
		{
			OkResult<PizzaResponse> okResult => Ok(okResult.Value),

			NotFoundResult<PizzaResponse> => NotFound(),

			_ => BadRequest()
		};
	}

	// POST api/Pizzas
	[HttpPost]
	public async Task<ActionResult<PizzaResponse>> PostPizza(PizzaCreateDto pizzaDto)
	{
		var result = await _service.CreatePizzaAsync(pizzaDto);

		return result switch
		{
			OkResult<PizzaResponse> { Value: var pizza } => CreatedAtAction(
				nameof(GetPizza), new { id = pizza.Id }, pizza
			),

			NotFoundResult<PizzaResponse> => NotFound(),

			DuplicateResult<PizzaResponse> dupeResult => Conflict(new
			{
				Message = $"A pizza with the name '{pizzaDto.Name}' already exists.",
				ErrorCode = "DUPLICATE_NAME",
				ConflictingResource = dupeResult.ConflictValue,
			}),

			InvalidToppingsResult badResult => BadRequest(new
			{
				Message = $"Invalid topping IDs: {string.Join(", ", badResult.InvalidToppingIds)}",
				ErrorCode = "INVALID_TOPPING_IDS",
				ToppingIds = badResult.InvalidToppingIds
			}),

			_ => BadRequest()
		};
	}

	// PUT api/Pizzas/1
	[HttpPut("{id}")]
	public async Task<IActionResult> PutPizza(int id, PizzaUpdateDto pizzaDto)
	{
		var result = await _service.UpdatePizzaAsync(id, pizzaDto);

		return result switch
		{
			OkResult<PizzaResponse> { Value: var pizza } => CreatedAtAction(
				nameof(GetPizza), new { id = pizza.Id }, pizza
			),

			NotFoundResult<PizzaResponse> => NotFound(),

			DuplicateResult<PizzaResponse> dupeResult => Conflict(new
			{
				Message = $"A pizza with the name '{pizzaDto.Name}' already exists.",
				ErrorCode = "DUPLICATE_NAME",
				ConflictingResource = dupeResult.ConflictValue,
			}),

			InvalidToppingsResult badResult => BadRequest(new
			{
				Message = $"Invalid topping IDs: {string.Join(", ", badResult.InvalidToppingIds)}",
				ErrorCode = "INVALID_TOPPING_IDS",
				ToppingIds = badResult.InvalidToppingIds
			}),

			_ => BadRequest()
		};
	}

	// DELETE api/Pizzas/1
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeletePizza(int id)
	{
		var result = await _service.DeletePizzaAsync(id);

		return result switch
		{
			{ IsSuccess: true } => NoContent(),

			NotFoundResult<PizzaResponse> => NotFound(),

			_ => BadRequest()
		};
	}
}
