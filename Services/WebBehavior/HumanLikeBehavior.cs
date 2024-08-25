using OpenQA.Selenium;

namespace CarsLoader.Services.WebBehavior;

public class HumanLikeBehavior
{
	public static async Task PerformRandomActions(IWebDriver driver, int minimumDelayMs, int maximumDelayMs)
	{
		var random = new Random();

		var iterations = random.Next(5, 10);
		for (var i = 0; i < iterations; i++)
		{
			await RandomScrolling.PerformRandomScrolling(driver, minimumDelayMs, maximumDelayMs);
			await RandomClicks.PerformRandomClicks(driver, minimumDelayMs, maximumDelayMs);
		}
	}
}