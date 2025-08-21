using System.ComponentModel.DataAnnotations;

namespace PizzaManagerAPI.Models;

public class Topping
{
	public int Id { get; set; }
	public required string Name { get; set; }
}

public class ToppingResponse
{
	public int Id { get; set; }
	public required string Name { get; set; }
}

public class ToppingPostDto
{
	[Required]
	[StringLength(100, MinimumLength = 2)]
	public required string Name { get; set; }
}

public class ToppingPutDto
{
	[Required]
	[StringLength(100, MinimumLength = 2)]
	public required string Name { get; set; }
}
