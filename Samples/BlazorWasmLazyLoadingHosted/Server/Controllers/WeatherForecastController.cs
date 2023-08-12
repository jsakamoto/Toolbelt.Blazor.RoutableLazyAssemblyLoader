using BlazorWasmApp.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWasmApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecast weatherForecast;

    public WeatherForecastController(IWeatherForecast weatherForecast)
    {
        this.weatherForecast = weatherForecast;
    }

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        return await this.weatherForecast.GetForecastAsync();
    }
}
