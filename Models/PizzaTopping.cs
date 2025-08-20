namespace PizzaManagerAPI.Models;

public class PizzaTopping
{
	public int PizzaId { get; set; }
	public int ToppingId { get; set; }

	public Pizza Pizza { get; set; } = null!;
	public Topping Topping { get; set; } = null!;
}
