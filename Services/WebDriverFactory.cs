using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CarsLoader.Services;

public sealed class WebDriverFactory : IWebDriverFactory
{
	private readonly List<string> _userAgents =
	[
		"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.5735.110 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:102.0) Gecko/20100101 Firefox/102.0",
		"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.1.2 Safari/605.1.15",
		"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.5735.110 Safari/537.36 Edg/114.0.1823.58",
		"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.5735.110 Safari/537.36 OPR/98.0.4759.39"
	];

	private readonly List<string> _chromeArguments = 
	[
		"--disable-blink-features=AutomationControlled", 
		"--headless", 
		"--lang=en"
	];

	private WebDriver _webDriver;
	
	public WebDriver CreateDriver()
	{
		var options = GetChromeOptions();
		_webDriver = new ChromeDriver(options);
		_webDriver.Manage().Window.Maximize();

		return _webDriver;
	}
	
	private ChromeOptions GetChromeOptions()
	{
		var options = new ChromeOptions();
		options.AddExcludedArgument("enable-automation");
		options.AddArguments(_chromeArguments);
		
		var index = Random.Shared.Next(_userAgents.Count);
		var randomUserAgent = _userAgents[index];
		options.AddArgument($"user-agent={randomUserAgent}");

		return options;
	}

	public void Dispose()
	{
		_webDriver.Quit();
	}
}