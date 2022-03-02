namespace OpenWeather.Domain
{
    public interface ITemperatureRepository
    {
        Task<Temperature> GetTemperatureByDateAndTimeAsync(DateTime date);
        Task StoreTemperatureAsync(Temperature temperature);
    }
}
