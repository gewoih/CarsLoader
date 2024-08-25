using CarsLoader.Infrastructure;
using CarsLoader.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

builder.Services.AddDbContext<CarsContext>(options => options.UseNpgsql("Host=localhost;Database=cars_loader;username=postgres;password=24042001Nr;"));

builder.Services.AddSingleton<IWebDriverFactory, WebDriverFactory>();
builder.Services.AddHostedService<CarsLoaderService>();
builder.Services.AddHostedService<CarsImagesLoaderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.Run();