using PizzaManagerAPI.Models;
using PizzaManagerAPI.Services.Results;

namespace PizzaManagerAPI.Services;

public interface IPizzaService
{
	Task<IEnumerable<PizzaResponse>> GetAllPizzasAsync();
	Task<Result<PizzaResponse>> GetPizzaByIdAsync(int id);
	Task<Result<PizzaResponse>> CreatePizzaAsync(PizzaCreateDto dto);
	Task<Result<PizzaResponse>> UpdatePizzaAsync(int id, PizzaUpdateDto dto);
	Task<Result<PizzaResponse>> DeletePizzaAsync(int id);
}
