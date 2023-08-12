using System.Net.Http.Json;
using BlazorWasmApp.Shared;

namespace BlazorWasmApp.Client.Services;

public class ClientWeatherForecast : IWeatherForecast
{
    private readonly HttpClient httpClient;

    public ClientWeatherForecast(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async ValueTask<IEnumerable<WeatherForecast>> GetForecastAsync()
    {
        return await this.httpClient.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast") ?? Enumerable.Empty<WeatherForecast>();
    }
}
