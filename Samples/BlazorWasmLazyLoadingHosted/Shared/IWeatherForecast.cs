namespace BlazorWasmApp.Shared;

public interface IWeatherForecast
{
    ValueTask<IEnumerable<WeatherForecast>> GetForecastAsync();
}
