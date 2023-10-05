using Microsoft.AspNetCore.Mvc;
using Prometheus;

namespace DotNetDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly Histogram ForecastDuration = Metrics.CreateHistogram("demo_forecast_generation_duration_seconds", "Histogram of weather forecast generation durations.");

        private static readonly Counter FailedForecastGeneration = Metrics.CreateCounter("demo_forecast_generation_failed_total", "Number of import operations that failed.");

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            using (ForecastDuration.NewTimer())
            {
                return FailedForecastGeneration.CountExceptions(() => GenerateForecast());
            }
        }

        private IEnumerable<WeatherForecast> GenerateForecast()
        {
            //throw new InvalidOperationException("Forecast unavailable");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}