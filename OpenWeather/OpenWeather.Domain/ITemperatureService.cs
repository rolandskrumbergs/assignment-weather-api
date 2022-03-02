namespace OpenWeather.Domain
{
    public interface ITemperatureService
    {
        Task<Temperature> GetTemperatureAsync(string date, string time);
        Task StoreTemperatureAsync();
    }
}
