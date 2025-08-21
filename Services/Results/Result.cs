namespace PizzaManagerAPI.Services.Results;

public abstract class Result<T>
{
	public abstract bool IsSuccess { get; }
	public bool IsFailure => !IsSuccess;
}
