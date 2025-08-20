using Microsoft.EntityFrameworkCore;
using PizzaManagerAPI.Models;

namespace PizzaManagerAPI.Data;

public class PizzaDbContext : DbContext
{
	public PizzaDbContext(DbContextOptions<PizzaDbContext> options) : base(options) { }

	public DbSet<Pizza> Pizzas => Set<Pizza>();
	public DbSet<Topping> Toppings => Set<Topping>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Pizza>().HasMany(p => p.Toppings).WithMany();
	}
}
