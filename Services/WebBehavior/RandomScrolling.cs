using OpenQA.Selenium;

namespace Encar;

public class RandomScrolling
{
	public static async Task PerformRandomScrolling(IWebDriver driver, int minimumDelayMs, int maximumDelayMs)
	{
		var random = new Random();

		try
		{
			var pageHeight = (long)((IJavaScriptExecutor)driver).ExecuteScript("return document.body.scrollHeight");
			var scrollPosition = random.Next(0, (int)pageHeight);
			((IJavaScriptExecutor)driver).ExecuteScript($"window.scrollTo(0, {scrollPosition});");
		}
		catch (Exception)
		{
			// ignored
		}

		var delay = random.Next(minimumDelayMs, maximumDelayMs);
		await Task.Delay(delay);
	}
}