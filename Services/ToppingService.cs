using Microsoft.EntityFrameworkCore;
using PizzaManagerAPI.Data;
using PizzaManagerAPI.Models;
using PizzaManagerAPI.Services.Results;

namespace PizzaManagerAPI.Services;

public class ToppingService : IToppingService
{
	private readonly PizzaDbContext _context;

	public ToppingService(PizzaDbContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<ToppingResponse>> GetAllToppingsAsync()
	{
		return await _context.Toppings
			.AsNoTracking()
			.Select(t => MapToResponse(t))
			.ToListAsync();
	}

	public async Task<Result<ToppingResponse>> GetToppingByIdAsync(int id)
	{
		var topping = await _context.Toppings.FindAsync(id);

		return topping != null
			? new OkResult<ToppingResponse>(MapToResponse(topping))
			: new NotFoundResult<ToppingResponse>();
	}

	public async Task<Result<ToppingResponse>> CreateToppingAsync(ToppingCreateDto toppingDto)
	{
		var dupe = await FindToppingDuplicate(toppingDto.Name);

		if (dupe != null)
		{
			return new DuplicateResult<ToppingResponse>(MapToResponse(dupe));
		}

		var topping = new Topping
		{
			Name = toppingDto.Name,
		};

		if (_context.Database.IsInMemory())
		{
			// autoincrement, for testing with in-memory database
			topping.Id = (await _context.Toppings.MaxAsync(t => (int?)t.Id) ?? 0) + 1;
		}

		_context.Toppings.Add(topping);

		await _context.SaveChangesAsync();

		return new OkResult<ToppingResponse>(MapToResponse(topping));
	}

	public async Task<Result<ToppingResponse>> UpdateToppingAsync(int id, ToppingUpdateDto toppingDto)
	{
		var topping = await _context.Toppings.FindAsync(id);

		if (topping == null)
		{
			return new NotFoundResult<ToppingResponse>();
		}

		var dupe = await FindToppingDuplicate(toppingDto.Name, id);

		if (dupe != null)
		{
			return new DuplicateResult<ToppingResponse>(MapToResponse(dupe));
		}

		topping.Name = toppingDto.Name;

		_context.Entry(topping).State = EntityState.Modified;

		await _context.SaveChangesAsync();

		return new OkResult<ToppingResponse>(MapToResponse(topping));
	}

	public async Task<Result<ToppingResponse>> DeleteToppingAsync(int id)
	{
		var topping = await _context.Toppings.FindAsync(id);

		if (topping == null)
		{
			return new NotFoundResult<ToppingResponse>();
		}

		if (await _context.PizzaToppings.AnyAsync(pt => pt.ToppingId == id))
		{
			return new ToppingInUseResult(
				await _context.PizzaToppings
					.AsNoTracking()
					.Where(pt => pt.ToppingId == id)
					.Select(pt => pt.PizzaId)
					.Distinct()
					.ToListAsync()
			);
		}

		var toppingResponse = MapToResponse(topping);

		_context.Toppings.Remove(topping);

		await _context.SaveChangesAsync();

		return new OkResult<ToppingResponse>(toppingResponse);
	}

	private async Task<Topping?> FindToppingDuplicate(string name, int? id = null)
	{
		bool isDupe = await _context.Toppings.AnyAsync(t =>
			(id == null || t.Id != id) && t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
		);

		if (!isDupe)
		{
			return null;
		}

		// doing a second query is slower, but i assume this is a far less common case
		return await _context.Toppings
			.AsNoTracking()
			.FirstOrDefaultAsync(t =>
				t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
			);
	}

	private static ToppingResponse MapToResponse(Topping topping)
	{
		return new ToppingResponse
		{
			Id = topping.Id,
			Name = topping.Name,
		};
	}
}
