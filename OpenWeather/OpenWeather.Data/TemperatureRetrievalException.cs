using System.Runtime.Serialization;

namespace OpenWeather.Data
{
    internal class TemperatureRetrievalException : Exception
    {
        public TemperatureRetrievalException()
        {
        }

        public TemperatureRetrievalException(string? message) : base(message)
        {
        }

        public TemperatureRetrievalException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TemperatureRetrievalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
