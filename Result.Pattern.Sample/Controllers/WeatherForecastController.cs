using Microsoft.AspNetCore.Mvc;
using Result.Pattern.Sample.Extensions;
using Result.Pattern.Sample.Results;

namespace Result.Pattern.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    [HttpGet]
    public IActionResult Get()
    {
        var result = GetWeatherForecast();
        return result.ToObjectResult();
    }

    [HttpGet("{name}")]
    public IActionResult Get(string name)
    {
        var result = GetByNameWeatherForecast(name);
        return result.ToObjectResult();
    }


    private static Result<IEnumerable<WeatherForecast>> GetWeatherForecast()
    {
        var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();

        return Result<IEnumerable<WeatherForecast>>.Success(result);
    }

    private static Result<IEnumerable<WeatherForecast>> GetByNameWeatherForecast(string name)
    {
        var result = Summaries.Where(s => s.Contains(name)).Select(s => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = s
        })
        .ToArray();

        return Result<IEnumerable<WeatherForecast>>.Success(result);
    }
}
