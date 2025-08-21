namespace PizzaManagerAPI.Services.Results;

public class NotFoundResult<T> : Result<T>
{
	public override bool IsSuccess => false;
}
