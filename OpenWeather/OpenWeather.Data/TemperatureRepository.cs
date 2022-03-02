using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using OpenWeather.Domain;
using System.Net;

namespace OpenWeather.Data
{
    public class TemperatureRepository : ITemperatureRepository
    {
        private readonly TableClient _temperatureTable;

        public TemperatureRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("TableStorage");
            
            var tableService = new TableServiceClient(connectionString);

            _temperatureTable = tableService.GetTableClient("temperature");
            _temperatureTable.CreateIfNotExists();
        }

        public async Task<Temperature> GetTemperatureByDateAndTimeAsync(DateTime date)
        {
            var partitionKey = GetPartitionKeyFromDate(date);
            var rowKey = GetRowKeyFromDate(date);

            Response<TableEntity>? temperatureEntity = null;

            try
            {
                temperatureEntity = await _temperatureTable.GetEntityAsync<TableEntity>(partitionKey, rowKey);
            }
            catch (RequestFailedException exception)
            {
                if (exception.Status != (int)HttpStatusCode.NotFound)
                {
                    return default;
                }
            }

            var temperature = temperatureEntity?.Value?.GetDouble("Temperature");
            var timestamp = temperatureEntity?.Value?.GetDateTime("Time");

            return new Temperature
            {
                DegreesInCelsium = temperature.HasValue ? temperature.Value : default,
                Timestamp = timestamp.HasValue ? timestamp.Value : default,
            };
        }

        public async Task StoreTemperatureAsync(Temperature temperature)
        {
            var partitionKey = GetPartitionKeyFromDate(temperature.Timestamp);
            var rowKey = GetRowKeyFromDate(temperature.Timestamp);

            var entity = new TableEntity(partitionKey, rowKey)
            {
                { "Temperature", temperature.DegreesInCelsium },
                { "Time", DateTime.SpecifyKind(temperature.Timestamp, DateTimeKind.Utc) }
            };

            await _temperatureTable.UpsertEntityAsync(entity);
        }

        private string GetPartitionKeyFromDate(DateTime date)
        {
            return $"{date.Year}_{date.Month}";
        }

        private string GetRowKeyFromDate(DateTime date)
        {
            return $"{date.Day}_{date.Hour}";
        }
    }
}