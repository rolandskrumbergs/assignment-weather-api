using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using OpenWeather.Data;
using OpenWeather.Domain;

[assembly: FunctionsStartup(typeof(OpenWeather.API.Startup))]

namespace OpenWeather.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            builder.Services.AddScoped<ITemperatureRepository, TemperatureRepository>();
            builder.Services.AddScoped<ITemperatureRetriever, TemperatureRetriever>();
            builder.Services.AddScoped<ITemperatureService, TemperatureService>();
        }
    }
}
