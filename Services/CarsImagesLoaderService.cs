using CarsLoader.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CarsLoader.Services;

public sealed class CarsImagesLoaderService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly string _imagesPath;

	public CarsImagesLoaderService(IServiceProvider serviceProvider, IConfiguration configuration)
	{
		_serviceProvider = serviceProvider;
		_imagesPath = configuration["ImagesPath"];
		Directory.CreateDirectory(_imagesPath);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var scope = _serviceProvider.CreateAsyncScope();
		while (!stoppingToken.IsCancellationRequested)
		{
			var context = scope.ServiceProvider.GetRequiredService<CarsContext>();
			var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
			var imagesToDownload = await context.Images
				.Where(image => !image.IsDownloaded)
				.OrderBy(image => image.Url)
				.ToListAsync(cancellationToken: stoppingToken);

			var imagesGroupedByCarId = imagesToDownload.GroupBy(image => image.CarId);
			var downloadTasks = imagesGroupedByCarId.SelectMany(group =>
			{
				return group.Select(async (carImage, index) =>
				{
					var imageBytes = await httpClient.GetByteArrayAsync(carImage.Url, stoppingToken);

					var imagePath = Path.Combine(_imagesPath, $"{carImage.CarId}_{index}.jpg");
					await File.WriteAllBytesAsync(imagePath, imageBytes, stoppingToken);

					carImage.Url = imagePath;
					carImage.IsDownloaded = true;
				});
			});

			await Task.WhenAll(downloadTasks);

			await context.SaveChangesAsync(stoppingToken);
			await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
		}
	}
}