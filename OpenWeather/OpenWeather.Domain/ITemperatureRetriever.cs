namespace OpenWeather.Domain
{
    public interface ITemperatureRetriever
    {
        Task<Temperature> GetTemperatureAsync();
    }
}
