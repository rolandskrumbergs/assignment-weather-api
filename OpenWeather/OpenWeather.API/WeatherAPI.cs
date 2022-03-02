using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using OpenWeather.Domain;
using System;
using System.Threading.Tasks;

namespace OpenWeather.API
{
    //  TURKU:
    //  "lat": 60.4517531
    //  "lon": 22.2670522
    //  NCRON https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-timer?tabs=csharp#ncrontab-examples
    public class WeatherAPI
    {
        private readonly ITemperatureService _temperatureService;

        public WeatherAPI(ITemperatureService temperatureService)
        {
            _temperatureService = temperatureService;
        }

        // 0 */1 * * * *
        // 0 0 * * * * -> Once at the top of every hour
        [FunctionName("Gather")]
        public async Task GatherAsync([TimerTrigger("0 */1 * * * *")]TimerInfo timer, ILogger logger)
        {
            logger.LogInformation("Retrieving temperature");

            await _temperatureService.StoreTemperatureAsync();
        }

        [FunctionName("Check")]
        public async Task<IActionResult> CheckAsync([HttpTrigger("get")] HttpRequest request, ILogger logger)
        {
            var date = request.Query["date"];
            var time = request.Query["time"];

            logger.LogInformation($"Invoking with date [{date}] and time [{time}]");

            try
            {
                var temperature = await _temperatureService.GetTemperatureAsync(date, time);

                if (temperature.Equals(default(Temperature)))
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(temperature);
            }
            catch (ArgumentException)
            {
                return new BadRequestObjectResult("Invalid date or time!");
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
