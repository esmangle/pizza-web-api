using System.ComponentModel.DataAnnotations;

namespace PizzaManagerAPI.Models;

public class Pizza
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public List<PizzaTopping> PizzaToppings { get; set; } = [];
}

public class PizzaResponse
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public List<PizzaToppingResponse> Toppings { get; set; } = [];
}

public class PizzaCreateDto
{
	[Required]
	[StringLength(100, MinimumLength = 2)]
	public required string Name { get; set; }

	public List<int> ToppingIds { get; set; } = [];
}

public class PizzaUpdateDto : IValidatableObject
{
	[StringLength(100, MinimumLength = 2)]
	public string? Name { get; set; } = null;

	public List<int>? ToppingIds { get; set; } = null;

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (Name == null && ToppingIds == null)
		{
			yield return new ValidationResult("No parameters provided");
		}
	}
}
