using PizzaManagerAPI.Models;

namespace PizzaManagerAPI.Services;

public interface IToppingService
{
	Task<IEnumerable<ToppingResponse>> GetAllToppingsAsync();
	Task<ToppingResponse?> GetToppingByIdAsync(int id);
	Task<ToppingResponse> CreateToppingAsync(ToppingCreateDto dto);
	Task<ToppingResponse> UpdateToppingAsync(int id, ToppingUpdateDto dto);
	Task DeleteToppingAsync(int id);
}
