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

public class PizzaPostDto
{
	public required string Name { get; set; }

	public List<int> ToppingIds { get; set; } = new();
}

public class PizzaPutDto
{
	public required string Name { get; set; }

	public List<int> ToppingIds { get; set; } = new();
}
