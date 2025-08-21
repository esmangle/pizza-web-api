using PizzaManagerAPI.Models;

namespace PizzaManagerAPI.Services.Results;

public class InvalidToppingsResult(IEnumerable<int> invalidToppingIds) : Result<PizzaResponse>
{
	public override bool IsSuccess => false;

	public IEnumerable<int> InvalidToppingIds { get; } = invalidToppingIds;
}
