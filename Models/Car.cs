namespace CarsLoader.Models;

public class Car
{
	public Guid Id { get; set; }
	public int EncarId { get; set; }
	public string Manufacturer { get; set; }
	public string Model { get; set; }
	public string Series { get; set; }
	public string Color { get; set; }
	public DateOnly ProductionDate { get; set; }
	public FuelType FuelType { get; set; }
	public int EngineCapacity { get; set; }
	public int Mileage { get; set; }
	public decimal WonPrice { get; set; }
	public List<CarImage> Images { get; set; }
}