using PizzaManagerAPI.Models;
using PizzaManagerAPI.Services.Results;

namespace PizzaManagerAPI.Services;

public interface IToppingService
{
	Task<IEnumerable<ToppingResponse>> GetAllToppingsAsync();
	Task<Result<ToppingResponse>> GetToppingByIdAsync(int id);
	Task<Result<ToppingResponse>> CreateToppingAsync(ToppingCreateDto dto);
	Task<Result<ToppingResponse>> UpdateToppingAsync(int id, ToppingUpdateDto dto);
	Task<Result<ToppingResponse>> DeleteToppingAsync(int id);
}
