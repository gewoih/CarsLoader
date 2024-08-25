using System.Text.RegularExpressions;
using System.Web;
using CarsLoader.Infrastructure;
using Encar;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CarsLoader.Services;

public sealed class CarsLoaderService : BackgroundService
{
	private readonly IWebDriverFactory _webDriverFactory;
	private readonly IServiceProvider _serviceProvider;

	private const string CarsUrl =
		"http://www.encar.com/fc/fc_carsearchlist.do?carType=for&searchType=model&TG.R=B#!%7B%22action%22%3A%22(And.Hidden.N._.CarType.N._.SellType.%EC%9D%BC%EB%B0%98._.(Or.FuelType.%EA%B0%80%EC%86%94%EB%A6%B0._.FuelType.%EB%94%94%EC%A0%A4.)_.Condition.Record._.Year.range(201900..)._.Mileage.range(..100000)._.Price.range(1000..).)%22%2C%22toggle%22%3A%7B%7D%2C%22layer%22%3A%22%22%2C%22sort%22%3A%22ModifiedDate%22%2C%22page%22%3A1%2C%22limit%22%3A250%2C%22searchKey%22%3A%22%22%2C%22loginCheck%22%3Afalse%7D";

	public CarsLoaderService(IWebDriverFactory webDriverFactory, IServiceProvider serviceProvider)
	{
		_webDriverFactory = webDriverFactory;
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var scope = _serviceProvider.CreateAsyncScope();
		while (!stoppingToken.IsCancellationRequested)
		{
			var context = scope.ServiceProvider.GetRequiredService<CarsContext>();

			using var webDriver = _webDriverFactory.CreateDriver();
			var carsUrls = await ExtractCarsUrlsAsync(webDriver, CarsUrl);
			carsUrls = carsUrls.Distinct().ToList();

			foreach (var carUrl in carsUrls)
			{
				try
				{
					var builtCar = await BuildCarFromUrlAsync(webDriver, carUrl);

					var isCarExists = await context.Cars
						.AsNoTracking()
						.AnyAsync(c => c.EncarId == builtCar.EncarId ||
						               (c.Manufacturer == builtCar.Manufacturer && c.Model == builtCar.Model &&
						                c.Series == builtCar.Series && c.Color == builtCar.Color &&
						                c.ProductionDate == builtCar.ProductionDate && c.Mileage == builtCar.Mileage),
							cancellationToken: stoppingToken);

					if (isCarExists)
						return;

					await context.Cars.AddAsync(builtCar, stoppingToken);
					await context.SaveChangesAsync(stoppingToken);

				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}

			await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
		}
	}

	private static async Task<Car> BuildCarFromUrlAsync(WebDriver webDriver, string carUrl)
	{
		await webDriver.Navigate().GoToUrlAsync(carUrl);

		var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(60));
		wait.Until(driver => driver.FindElements(By.Name("carDetail")).Count > 0);
		var form = webDriver.FindElement(By.Name("carDetail"));
		
		wait.Until(driver => driver.FindElements(By.TagName("input")).Count > 1);
		var inputFields = form.FindElements(By.TagName("input"));

		var car = new Car();
		var imagesUrls = ExtractCarImagesUrls(webDriver);
		car.Images = imagesUrls.Select(url => new CarImage { Url = url }).ToList();

		foreach (var input in inputFields)
		{
			var name = input.GetAttribute("name");
			var value = input.GetAttribute("value");

			switch (name)
			{
				case "carid":
					car.EncarId = int.Parse(value);
					break;
				case "mlg":
					car.Mileage = int.Parse(value);
					break;
				case "dsp":
					car.EngineCapacity = int.Parse(value);
					break;
				case "clr":
					car.Color = value;
					break;
				case "whatfuel":
					name = "Топливо";
					break;
				case "mnfcnm":
					car.Manufacturer = value;
					break;
				case "mdlnm":
					car.Model = value;
					break;
				case "clsheadnm":
					car.Series = value;
					break;
				case "dmndprc":
					car.WonPrice = decimal.Parse(value) * 10000;
					break;
				case "yr":
					var year = int.Parse(string.Concat(value.Take(4)));
					var month = int.Parse(string.Concat(value.TakeLast(2)));
					car.ProductionDate = new DateOnly(year, month, 1);
					break;
			}
		}

		return car;
	}

	private static async Task<List<string>> ExtractCarsUrlsAsync(WebDriver webDriver, string pageUrl)
	{
		await webDriver.Navigate().GoToUrlAsync(pageUrl);

		var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(60));
		wait.Until(driver => driver.FindElements(By.Id("sr_normal")).Count > 0);
		var tbody = webDriver.FindElement(By.Id("sr_normal"));

		wait.Until(driver => driver.FindElements(By.XPath(".//tr[@data-index]")).Count > 10);
		var carsRows = tbody.FindElements(By.XPath(".//tr[@data-index]"));

		var carsUrls = new List<string>();
		var regex = new Regex(@"carid=(\d+)");
		foreach (var carRow in carsRows)
		{
			var linkElement = carRow.FindElement(By.CssSelector("a.newLink._link"));
			var carLink = linkElement.GetAttribute("href");

			var carId = regex.Match(carLink).Groups[1].Value;
			carsUrls.Add(
				$"http://www.encar.com/dc/dc_cardetailview.do?pageid=dc_carsearch&listAdvType=pic&carid={carId}");
		}

		return carsUrls;
	}

	private static IEnumerable<string> ExtractCarImagesUrls(WebDriver webDriver)
	{
		var photoLinks = webDriver.FindElements(By.CssSelector("img.photo_s"));
		foreach (var linkElement in photoLinks)
		{
			var url = linkElement.GetAttribute("src");
			yield return RemoveQueryParameters(url, ["cg", "wtmk", "wtmkg", "wtmkw", "wtmkh", "t", "cw", "ch", "rh"]);
		}
	}
	
	private static string RemoveQueryParameters(string url, string[] parametersToRemove)
	{
		var uriBuilder = new UriBuilder(url);
		var query = HttpUtility.ParseQueryString(uriBuilder.Query);

		foreach (var param in parametersToRemove)
		{
			query.Remove(param);
		}

		uriBuilder.Query = query.ToString();
		return uriBuilder.ToString();
	}
}