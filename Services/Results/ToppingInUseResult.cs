using PizzaManagerAPI.Models;

namespace PizzaManagerAPI.Services.Results;

public class ToppingInUseResult(IEnumerable<int> pizzaIds) : Result<ToppingResponse>
{
	public override bool IsSuccess => false;

	public IEnumerable<int> PizzaIds { get; } = pizzaIds;
}
