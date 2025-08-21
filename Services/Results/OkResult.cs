namespace PizzaManagerAPI.Services.Results;

public class OkResult<T>(T value) : Result<T>
{
	public override bool IsSuccess => true;

	public T Value { get; } = value;
}
