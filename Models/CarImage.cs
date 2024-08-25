namespace CarsLoader.Models;

public class CarImage
{
	public Guid Id { get; set; }
	public Guid CarId { get; set; }
	public string Url { get; set; }
	public bool IsDownloaded { get; set; }
}