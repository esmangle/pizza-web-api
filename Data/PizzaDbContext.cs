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

		SeedExampleData(modelBuilder);
	}

	private void SeedExampleData(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Topping>().HasData(
			new Topping { Id = 1, Name = "Pepperoni" },
			new Topping { Id = 2, Name = "Pineapple" },
			new Topping { Id = 3, Name = "Ham" },
			new Topping { Id = 4, Name = "Bacon" },
			new Topping { Id = 5, Name = "Sausages" },
			new Topping { Id = 6, Name = "Mushrooms" }
		);

		modelBuilder.Entity<Pizza>().HasData(
			new Pizza { Id = 1, Name = "Pepperoni" },
			new Pizza { Id = 2, Name = "Hawaiian" },
			new Pizza { Id = 3, Name = "Meat Lover's" }
		);

		modelBuilder.Entity<PizzaTopping>().HasData(
			// Pepperoni
			new PizzaTopping { PizzaId = 1, ToppingId = 1 }, // Pepperoni
			// Hawaiian
			new PizzaTopping { PizzaId = 2, ToppingId = 2 }, // Pineapple
			new PizzaTopping { PizzaId = 2, ToppingId = 3 }, // Ham
			// Meat Lover's
			new PizzaTopping { PizzaId = 3, ToppingId = 1 }, // Pepperoni
			new PizzaTopping { PizzaId = 3, ToppingId = 3 }, // Ham
			new PizzaTopping { PizzaId = 3, ToppingId = 4 }, // Bacon
			new PizzaTopping { PizzaId = 3, ToppingId = 5 }  // Sausages
		);
	}
}
