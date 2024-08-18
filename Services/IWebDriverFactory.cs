using OpenQA.Selenium;

namespace CarsLoader.Services;

public interface IWebDriverFactory : IDisposable
{
	WebDriver CreateDriver();
}