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

    [HttpGet(Name = "GetWeatherForecast")]
    public IActionResult Get()
    {
        var result = GetWeatherForecast();
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
}
