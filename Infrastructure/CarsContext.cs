using CarsLoader.Models;
using Microsoft.EntityFrameworkCore;

namespace CarsLoader.Infrastructure;

public sealed class CarsContext : DbContext
{
	public DbSet<Car> Cars { get; set; }
	public DbSet<CarImage> Images { get; set; }

	public CarsContext(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Car>()
			.HasIndex(car => new { car.Manufacturer, car.Model, car.Series, car.ProductionDate, car.EngineCapacity, car.Color, car.Mileage })
			.IsUnique();

		modelBuilder.Entity<Car>()
			.HasIndex(car => car.EncarId)
			.IsUnique();
	}
}