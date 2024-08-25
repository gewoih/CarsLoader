using OpenQA.Selenium;

namespace CarsLoader.Services.WebBehavior;

public class RandomClicks
{
	public static async Task PerformRandomClicks(IWebDriver driver, int minimumDelayMs, int maximumDelayMs)
	{
		var random = new Random();

		var clickableElements = driver.FindElements(By.CssSelector("a, button, input[type='button'], input[type='submit']"));
		if (clickableElements.Count > 0)
		{
			var index = random.Next(clickableElements.Count);
			var elementToClick = clickableElements[index];

			try
			{
				elementToClick.Click();
			}
			catch (Exception)
			{
				// ignored
			}
		}
        
		var delay = random.Next(minimumDelayMs, maximumDelayMs);
		await Task.Delay(delay);
	}
}