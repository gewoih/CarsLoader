namespace CarsLoader.DTO;

public class CarsSearchFilter
{
	public string? Manufacturer { get; set; }
	public string? Model { get; set; }
	public string? Series { get; set; }
	public DateOnly? ProductionDateFrom { get; set; }
	public DateOnly? ProductionDateTo { get; set; }
	public int? EngineCapacityFrom { get; set; }
	public int? EngineCapacityTo { get; set; }
	public int? MileageFrom { get; set; }
	public int? MileageTo { get; set; }
	public decimal? WonPriceFrom { get; set; }
	public decimal? WonPriceTo { get; set; }
	public int Skip { get; set; } = 0;
	public int Take { get; set; } = 100;
}