namespace PizzaManagerAPI.Services.Results;

public class DuplicateResult<T>(T conflictValue) : Result<T>
{
	public override bool IsSuccess => false;

	public T ConflictValue { get; } = conflictValue;
}
