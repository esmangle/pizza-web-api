namespace PizzaManagerAPI.Models;

public class PizzaTopping
{
	public int PizzaId { get; set; }
	public int ToppingId { get; set; }

	public Pizza Pizza { get; set; } = null!;
	public Topping Topping { get; set; } = null!;
}

public class PizzaToppingResponse
{
	public int Id { get; set; }
	public required string Name { get; set; }
}
