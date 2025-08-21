using PizzaManagerAPI.Models;

namespace PizzaManagerAPI.Services;

public interface IPizzaService
{
	Task<IEnumerable<PizzaResponse>> GetAllPizzasAsync();
	Task<PizzaResponse?> GetPizzaByIdAsync(int id);
	Task<PizzaResponse> CreatePizzaAsync(PizzaCreateDto dto);
	Task<PizzaResponse> UpdatePizzaAsync(int id, PizzaUpdateDto dto);
	Task DeletePizzaAsync(int id);
}
