using Microsoft.EntityFrameworkCore;
using PizzaManagerAPI.Data;
using PizzaManagerAPI.Models;
using PizzaManagerAPI.Services.Results;

namespace PizzaManagerAPI.Services;

public class PizzaService : IPizzaService
{
	private readonly PizzaDbContext _context;

	public PizzaService(PizzaDbContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<PizzaResponse>> GetAllPizzasAsync()
	{
		return await _context.Pizzas
			.AsNoTracking()
			.Include(p => p.PizzaToppings)
			.ThenInclude(pt => pt.Topping)
			.Select(p => MapToResponse(p))
			.ToListAsync();
	}

	public async Task<Result<PizzaResponse>> GetPizzaByIdAsync(int id)
	{
		var pizza = await _context.Pizzas
			.AsNoTracking()
			.Include(p => p.PizzaToppings)
			.ThenInclude(pt => pt.Topping)
			.FirstOrDefaultAsync(p => p.Id == id);

		return pizza != null
			? new OkResult<PizzaResponse>(MapToResponse(pizza))
			: new NotFoundResult<PizzaResponse>();
	}

	public async Task<Result<PizzaResponse>> CreatePizzaAsync(PizzaCreateDto pizzaDto)
	{
		var (dupe, invalidToppingIds) = await ValidatePizzaAsync(
			pizzaDto.Name, pizzaDto.ToppingIds
		);

		if (dupe != null)
		{
			return new DuplicateResult<PizzaResponse>(MapToResponse(dupe));
		}

		if (invalidToppingIds.Any())
		{
			return new InvalidToppingsResult(invalidToppingIds);
		}

		using var transaction = await _context.Database.BeginTransactionAsync();

		try
		{
			var pizza = new Pizza { Name = pizzaDto.Name };

			if (_context.Database.IsInMemory())
			{
				// autoincrement, for testing with in-memory database
				pizza.Id = (await _context.Pizzas.MaxAsync(p => (int?)p.Id) ?? 0) + 1;
			}

			_context.Pizzas.Add(pizza);

			_context.PizzaToppings.AddRange(
				pizzaDto.ToppingIds.Select(toppingId =>
					new PizzaTopping
					{
						PizzaId = pizza.Id,
						ToppingId = toppingId,
					}
				)
			);

			await _context.SaveChangesAsync();
			await transaction.CommitAsync();

			await _context.Entry(pizza)
				.Collection(p => p.PizzaToppings)
				.Query()
				.Include(pt => pt.Topping)
				.LoadAsync();

			return new OkResult<PizzaResponse>(MapToResponse(pizza));
		}
		catch
		{
			await transaction.RollbackAsync();
			throw;
		}
	}

	public async Task<Result<PizzaResponse>> UpdatePizzaAsync(int id, PizzaUpdateDto pizzaDto)
	{
		var pizza = await _context.Pizzas
			.Include(p => p.PizzaToppings)
			.FirstOrDefaultAsync(p => p.Id == id);

		if (pizza == null)
		{
			return new NotFoundResult<PizzaResponse>();
		}

		var (dupe, invalidToppingIds) = await ValidatePizzaAsync(
			pizzaDto.Name, pizzaDto.ToppingIds, id
		);

		if (dupe != null)
		{
			return new DuplicateResult<PizzaResponse>(MapToResponse(dupe));
		}

		if (invalidToppingIds.Any())
		{
			return new InvalidToppingsResult(invalidToppingIds);
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
					new PizzaTopping
					{
						PizzaId = pizza.Id,
						ToppingId = toppingId,
					}
				);

			_context.PizzaToppings.RemoveRange(toppingsToRemove);
			_context.PizzaToppings.AddRange(toppingsToAdd);

			await _context.SaveChangesAsync();
			await transaction.CommitAsync();

			await _context.Entry(pizza)
				.Collection(p => p.PizzaToppings)
				.Query()
				.Include(pt => pt.Topping)
				.LoadAsync();

			return new OkResult<PizzaResponse>(MapToResponse(pizza));
		}
		catch (DbUpdateConcurrencyException)
		{
			await transaction.RollbackAsync();

			if (!_context.Pizzas.Any(e => e.Id == id))
			{
				return new NotFoundResult<PizzaResponse>();
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

	public async Task<Result<PizzaResponse>> DeletePizzaAsync(int id)
	{
		var pizza = await _context.Pizzas
			.AsNoTracking()
			.Include(p => p.PizzaToppings)
			.ThenInclude(pt => pt.Topping)
			.FirstOrDefaultAsync(p => p.Id == id);

		if (pizza == null)
		{
			return new NotFoundResult<PizzaResponse>();
		}

		using var transaction = await _context.Database.BeginTransactionAsync();

		try
		{
			var pizzaResponse = MapToResponse(pizza);

			_context.PizzaToppings.RemoveRange(pizza.PizzaToppings);
			_context.Pizzas.Remove(pizza);

			await _context.SaveChangesAsync();
			await transaction.CommitAsync();

			return new OkResult<PizzaResponse>(pizzaResponse);
		}
		catch
		{
			await transaction.RollbackAsync();
			throw;
		}
	}

	private async Task<(Pizza? dupe, IEnumerable<int> invalidToppingIds)> ValidatePizzaAsync(
		string name, IEnumerable<int> toppingIds, int? id = null)
	{
		bool isDupe = await _context.Pizzas.AnyAsync(p =>
			(id == null || p.Id != id) && p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
		);

		// doing a second query is slower, but i assume this is a far less common case
		var dupeTask = isDupe ? _context.Pizzas
			.AsNoTracking()
			.Include(p => p.PizzaToppings)
			.ThenInclude(pt => pt.Topping)
			.FirstOrDefaultAsync(p =>
				p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
			)
			: null;

		var toppingsTask = _context.Toppings
			.AsNoTracking()
			.Where(t => toppingIds.Contains(t.Id))
			.Select(t => t.Id)
			.ToListAsync();

		await (dupeTask == null ? toppingsTask : Task.WhenAll(dupeTask, toppingsTask));

		return (
			dupeTask?.Result,
			toppingIds.Except(toppingsTask.Result).ToList()
		);
	}

	private static PizzaResponse MapToResponse(Pizza pizza)
	{
		return new PizzaResponse
		{
			Id = pizza.Id,
			Name = pizza.Name,
			Toppings = pizza.PizzaToppings.Select(
				pt => new PizzaToppingResponse
				{
					Id = pt.Topping.Id,
					Name = pt.Topping.Name
				}
			).ToList()
		};
	}
}
