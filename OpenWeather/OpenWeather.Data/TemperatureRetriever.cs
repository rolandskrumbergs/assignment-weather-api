using Microsoft.Extensions.Configuration;
using OpenWeather.Domain;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace OpenWeather.Data
{
    public class TemperatureRetriever : ITemperatureRetriever
    {
        private const string Longitude = "22.2670522";
        private const string Latitude = "60.4517531";
        private const string BaseAddress = "http://api.openweathermap.org/data/2.5/weather";
        private const string Units = "metric";

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TemperatureRetriever(
            IHttpClientFactory httpClientFactory, 
            IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        public async Task<Temperature> GetTemperatureAsync()
        {
            var appId = _configuration["OpenWeatherAppId"];

            var url = $"{BaseAddress}?lat={Latitude}&lon={Longitude}&units={Units}&appid={appId}";

            var result = await _httpClient.GetFromJsonAsync<TemperatureApiResponse>(url);

            if (result == null || result.Main == null)
            {
                throw new TemperatureRetrievalException();
            } 

            if (result.UnixEpochInUTC == null)
            {
                throw new TemperatureRetrievalException();
            }

            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(result.UnixEpochInUTC.Value).ToLocalTime();

            return new Temperature
            {
                DegreesInCelsium = result.Main.Temperature.HasValue ? result.Main.Temperature.Value : default,
                Timestamp = dateTime
            };
        }
    }

    public class TemperatureApiResponse
    {
        public TemperatureApiMain? Main { get; set; }

        [JsonPropertyName("dt")]
        public int? UnixEpochInUTC { get; set; }
    }

    public class TemperatureApiMain
    {
        [JsonPropertyName("temp")]
        public double? Temperature { get; set; }
    }
}
