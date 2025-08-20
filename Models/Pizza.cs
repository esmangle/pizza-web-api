namespace PizzaManagerAPI.Models;

public class Pizza
{
 	public int Id { get; set; }
	public required string Name { get; set; }
	public List<PizzaTopping> PizzaToppings { get; set; } = new();
}
