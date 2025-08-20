namespace PizzaManagerAPI.Models;

public class Topping
{
	public int Id { get; set; }
	public required string Name { get; set; }
}

public class ToppingPostDto
{
	public required string Name { get; set; }
}

public class ToppingPutDto
{
	public required string Name { get; set; }
}
