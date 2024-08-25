using CarsLoader.DTO;
using CarsLoader.Models;

namespace CarsLoader.Services;

public interface ICarsService
{
	Task<(List<Car>, int)> GetAsync(CarsSearchFilter filter);
	Task<List<string>> GetAllManufacturers();
	Task<List<string>> GetModels(string manufacturer);
	Task<List<string>> GetSeries(string model);
}