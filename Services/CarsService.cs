using CarsLoader.DTO;
using CarsLoader.Infrastructure;
using CarsLoader.Models;
using Microsoft.EntityFrameworkCore;

namespace CarsLoader.Services;

public sealed class CarsService : ICarsService
{
	private readonly CarsContext _context;

	public CarsService(CarsContext context)
	{
		_context = context;
	}

	public async Task<(List<Car>, int)> GetAsync(CarsSearchFilter filter)
	{
		var carsSearchQuery = _context.Cars
			.Include(car => car.Images)
			.Where(car => car.Images.All(image => image.IsDownloaded) && car.IsTranslated)
			.AsQueryable();

		if (!string.IsNullOrEmpty(filter.Manufacturer))
			carsSearchQuery = carsSearchQuery.Where(car =>
				car.Manufacturer.ToLower().Equals(filter.Manufacturer.ToLower()));

		if (!string.IsNullOrEmpty(filter.Model))
			carsSearchQuery = carsSearchQuery.Where(car => car.Model == filter.Model);

		if (!string.IsNullOrEmpty(filter.Series))
			carsSearchQuery = carsSearchQuery.Where(car => car.Series == filter.Series);

		if (filter.ProductionDateFrom.HasValue)
			carsSearchQuery = carsSearchQuery.Where(car => car.ProductionDate >= filter.ProductionDateFrom.Value);

		if (filter.ProductionDateTo.HasValue)
			carsSearchQuery = carsSearchQuery.Where(car => car.ProductionDate <= filter.ProductionDateTo.Value);

		if (filter.EngineCapacityFrom.HasValue)
			carsSearchQuery = carsSearchQuery.Where(car => car.EngineCapacity >= filter.EngineCapacityFrom.Value);

		if (filter.EngineCapacityTo.HasValue)
			carsSearchQuery = carsSearchQuery.Where(car => car.EngineCapacity <= filter.EngineCapacityTo.Value);

		if (filter.MileageFrom.HasValue)
			carsSearchQuery = carsSearchQuery.Where(car => car.Mileage >= filter.MileageFrom.Value);

		if (filter.MileageTo.HasValue)
			carsSearchQuery = carsSearchQuery.Where(car => car.Mileage <= filter.MileageTo.Value);

		if (filter.WonPriceFrom.HasValue)
			carsSearchQuery = carsSearchQuery.Where(car => car.WonPrice >= filter.WonPriceFrom.Value);

		if (filter.WonPriceTo.HasValue)
			carsSearchQuery = carsSearchQuery.Where(car => car.WonPrice <= filter.WonPriceTo.Value);

		var totalCars = await carsSearchQuery.CountAsync();
		var foundCars = await carsSearchQuery
			.Skip(filter.Skip)
			.Take(filter.Take)
			.ToListAsync();
			
		foreach (var car in foundCars)
		{
			car.Images = car.Images.OrderBy(image => image.Url).ToList();
		}

		return (foundCars, totalCars);
	}

	public async Task<Car?> GetAsync(Guid id)
	{
		var foundCar = await _context.Cars
			.Include(car => car.Images)
			.FirstOrDefaultAsync(car => car.Id == id);

		if (foundCar is not null)
		{
			foundCar.Images = foundCar.Images.OrderBy(image => image.Url).ToList();
		}

		return foundCar;
	}

	public async Task<List<string>> GetAllManufacturers()
	{
		return await _context.Cars
			.AsNoTracking()
			.Where(car => car.IsTranslated)
			.Select(car => car.Manufacturer)
			.Distinct()
			.ToListAsync();
	}

	public async Task<List<string>> GetModels(string manufacturer)
	{
		return await _context.Cars
			.AsNoTracking()
			.Where(car => car.IsTranslated)
			.Where(car => car.Manufacturer.ToLower() == manufacturer.ToLower())
			.Select(car => car.Model)
			.Distinct()
			.ToListAsync();
	}

	public async Task<List<string>> GetSeries(string model)
	{
		return await _context.Cars
			.AsNoTracking()
			.Where(car => car.IsTranslated)
			.Where(car => car.Model.ToLower() == model.ToLower())
			.Select(car => car.Series)
			.Distinct()
			.ToListAsync();
	}
}