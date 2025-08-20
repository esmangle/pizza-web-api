using Microsoft.EntityFrameworkCore;
using PizzaManagerAPI.Models;

namespace PizzaManagerAPI.Data;

public class PizzaDbContext : DbContext
{
	public PizzaDbContext(DbContextOptions<PizzaDbContext> options) : base(options) { }

	public DbSet<Topping> Toppings => Set<Topping>();
	public DbSet<Pizza> Pizzas => Set<Pizza>();
	public DbSet<PizzaTopping> PizzaToppings => Set<PizzaTopping>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<PizzaTopping>(e =>
		{
			e.HasKey(pt => new { pt.PizzaId, pt.ToppingId });

			e.HasOne(pt => pt.Pizza)
				.WithMany(p => p.PizzaToppings)
				.HasForeignKey(pt => pt.PizzaId);

			e.HasOne(pt => pt.Topping)
				.WithMany()
				.HasForeignKey(pt => pt.ToppingId);
		});
	}
}
