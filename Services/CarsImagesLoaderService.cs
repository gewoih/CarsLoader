using CarsLoader.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CarsLoader.Services;

public sealed class CarsImagesLoaderService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	private const string ImagesPath = "C:/CarsLoader";

	public CarsImagesLoaderService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var scope = _serviceProvider.CreateAsyncScope();
		while (!stoppingToken.IsCancellationRequested)
		{
			var context = scope.ServiceProvider.GetRequiredService<CarsContext>();
			var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
			var imagesToDownload = await context.Images
				.Where(image => !image.IsDownloaded && image.CarId == Guid.Parse("1ad7c99c-8aab-42da-945b-abbd2e904ce3"))
				.OrderBy(image => image.Url)
				.ToListAsync(cancellationToken: stoppingToken);

			for (var index = 0; index < imagesToDownload.Count; index++)
			{
				var carImage = imagesToDownload[index];
				var imageBytes = await httpClient.GetByteArrayAsync(carImage.Url, stoppingToken);
				Directory.CreateDirectory(ImagesPath);

				var imagePath = $"{ImagesPath}/{carImage.CarId}_{index}.jpg";
				await File.WriteAllBytesAsync(imagePath, imageBytes, stoppingToken);

				carImage.Url = imagePath;
				carImage.IsDownloaded = true;
			}

			await context.SaveChangesAsync(stoppingToken);
			await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
		}
	}
}