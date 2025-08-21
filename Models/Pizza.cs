using System.ComponentModel.DataAnnotations;

namespace PizzaManagerAPI.Models;

public class Pizza
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public List<PizzaTopping> PizzaToppings { get; set; } = new();
}

public class PizzaResponse
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public List<PizzaToppingResponse> Toppings { get; set; } = new();
}

public class PizzaCreateDto
{
	[Required]
	[StringLength(100, MinimumLength = 2)]
	public required string Name { get; set; }

	public List<int> ToppingIds { get; set; } = new();
}

public class PizzaUpdateDto
{
	[Required]
	[StringLength(100, MinimumLength = 2)]
	public required string Name { get; set; }

	public List<int> ToppingIds { get; set; } = new();
}
