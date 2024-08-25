using CarsLoader.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CarsLoader.Services;

public sealed class CarsTranslationService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;

	public CarsTranslationService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await using var scope = _serviceProvider.CreateAsyncScope();
		while (!stoppingToken.IsCancellationRequested)
		{
			var context = scope.ServiceProvider.GetRequiredService<CarsContext>();
			var notTranslatedCars = await context.Cars.Where(car => !car.IsTranslated).ToListAsync(cancellationToken: stoppingToken);
			if (notTranslatedCars.Count < 100)
			{
				var manufacturers = await notTranslatedCars.Select(car => car.Manufacturer).TranslateToEnglish();
				var models = await notTranslatedCars.Select(car => car.Model).TranslateToEnglish();
				var series = await notTranslatedCars.Select(car => car.Series).TranslateToEnglish();
				var colors = await notTranslatedCars.Select(car => car.Color).TranslateToEnglish();

				for (var i = 0; i < notTranslatedCars.Count; i++)
				{
					notTranslatedCars[i].Manufacturer =
						manufacturers[i].Equals("benz", StringComparison.InvariantCultureIgnoreCase)
							? "Mercedes-Benz"
							: manufacturers[i];

					notTranslatedCars[i].Model = char.ToUpper(models[i][0]) + models[i][1..];
					notTranslatedCars[i].Series = char.ToUpper(series[i][0]) + series[i][1..];
					notTranslatedCars[i].Color = char.ToUpper(colors[i][0]) + colors[i][1..];

					notTranslatedCars[i].IsTranslated = true;
				}

				await context.SaveChangesAsync(stoppingToken);
			}

			await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
		}
	}
}