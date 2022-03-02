using System.Runtime.Serialization;

namespace OpenWeather.Domain
{
    public class InvalidDateTimeException : Exception
    {
        public InvalidDateTimeException()
        {
        }

        public InvalidDateTimeException(string? message) : base(message)
        {
        }

        public InvalidDateTimeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidDateTimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
