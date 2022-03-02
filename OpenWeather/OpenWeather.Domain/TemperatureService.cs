namespace OpenWeather.Domain
{
    public class TemperatureService : ITemperatureService
    {
        private readonly ITemperatureRepository _temperatureRepository;
        private readonly ITemperatureRetriever _temperatureRetriever;

        public TemperatureService(
            ITemperatureRepository temperatureRepository,
            ITemperatureRetriever temperatureRetriever)
        {
            _temperatureRepository = temperatureRepository;
            _temperatureRetriever = temperatureRetriever;
        }

        public async Task<Temperature> GetTemperatureAsync(string date, string time)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                throw new ArgumentException("Date is empty!");
            }

            if (string.IsNullOrWhiteSpace(time))
            {
                throw new ArgumentException("Time is empty!");
            }

            var dateTime = ParseDateTime(date, time);

            return await _temperatureRepository.GetTemperatureByDateAndTimeAsync(dateTime);
        }

        public async Task StoreTemperatureAsync()
        {
            var currentTemperatureInTurku = await _temperatureRetriever.GetTemperatureAsync();

            await _temperatureRepository.StoreTemperatureAsync(currentTemperatureInTurku);
        }

        private DateTime ParseDateTime(string date, string time)
        {
            try
            {
                var dateSplit = date.Split('.');
                var year = int.Parse(dateSplit[2]);
                var month = int.Parse(dateSplit[1]);
                var day = int.Parse(dateSplit[0]);

                var timeSplit = time.Split(':');
                var hour = int.Parse(timeSplit[0]);
                var minute = int.Parse(timeSplit[1]);

                return new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc);
            }
            catch (Exception exception)
            {
                throw new InvalidDateTimeException($"Failed to parse date and time from date [{date}] and time [{time}]", exception);
            }
        }
    }
}
