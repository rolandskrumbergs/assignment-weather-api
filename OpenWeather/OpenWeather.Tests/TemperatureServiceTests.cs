using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenWeather.Domain;
using System;
using System.Threading.Tasks;

namespace OpenWeather.Tests
{
    [TestClass]
    public class TemperatureServiceTests
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Empty_Date_Throws_ArgumentException(string date)
        {
            var temperatureRepository = new Mock<ITemperatureRepository>();
            var temperatureRetriever = new Mock<ITemperatureRetriever>();

            var temperatureService = new TemperatureService(temperatureRepository.Object, temperatureRetriever.Object);

            temperatureService
                .Invoking(x => x.GetTemperatureAsync(date, "some time"))
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Date is empty!");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Empty_Time_Throws_ArgumentException(string time)
        {
            var temperatureRepository = new Mock<ITemperatureRepository>();
            var temperatureRetriever = new Mock<ITemperatureRetriever>();

            var temperatureService = new TemperatureService(temperatureRepository.Object, temperatureRetriever.Object);

            temperatureService
                .Invoking(x => x.GetTemperatureAsync("some date", time))
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Time is empty!");
        }

        [TestMethod]
        public async Task Valid_Date_And_Time_Returns_Temperature()
        {
            var date = "10.10.2021";
            var time = "10:00";

            var temperatureRepository = new Mock<ITemperatureRepository>();
            var temperatureRetriever = new Mock<ITemperatureRetriever>();

            var temperature = new Temperature
            {
                DegreesInCelsium = 20.8
            };

            var dateTime = new DateTime(2021, 10, 10, 10, 0, 0);
            temperatureRepository
                .Setup(x => x.GetTemperatureByDateAndTimeAsync(dateTime))
                .ReturnsAsync(temperature);

            var temperatureService = new TemperatureService(temperatureRepository.Object, temperatureRetriever.Object);

            var result = await temperatureService.GetTemperatureAsync(date, time);

            result.Should().BeEquivalentTo(temperature);
        }

        [TestMethod]
        [DataRow("1")]
        [DataRow(".10.2021")]
        [DataRow("10..2021")]
        [DataRow("10/10/2021")]
        [DataRow("2021.10.10")]
        public void Invalid_Date_And_Valid_Time_Throws_InvalidDateTimeException(string invalidDate)
        {
            var time = "10:00";

            var temperatureRepository = new Mock<ITemperatureRepository>();
            var temperatureRetriever = new Mock<ITemperatureRetriever>();

            var temperatureService = new TemperatureService(temperatureRepository.Object, temperatureRetriever.Object);

            temperatureService
                .Invoking(x => x.GetTemperatureAsync(invalidDate, time))
                .Should()
                .ThrowAsync<InvalidDateTimeException>()
                .WithMessage($"Failed to parse date and time from date [{invalidDate}] and time [{time}]");
        }

        [TestMethod]
        [DataRow("1")]
        [DataRow("10.00")]
        [DataRow("10.")]
        [DataRow(".00")]
        [DataRow("10-00")]
        public void Valid_Date_And_Invalid_Time_Throws_InvalidDateTimeException(string invalidTime)
        {
            var date = "10.10.2021";

            var temperatureRepository = new Mock<ITemperatureRepository>();
            var temperatureRetriever = new Mock<ITemperatureRetriever>();

            var temperatureService = new TemperatureService(temperatureRepository.Object, temperatureRetriever.Object);

            temperatureService
                .Invoking(x => x.GetTemperatureAsync(date, invalidTime))
                .Should()
                .ThrowAsync<InvalidDateTimeException>()
                .WithMessage($"Failed to parse date and time from date [{date}] and time [{invalidTime}]");
        }

        [TestMethod]
        public async Task Retrieved_Temperature_Stored_In_Repository()
        {
            var temperatureRepository = new Mock<ITemperatureRepository>();
            var temperatureRetriever = new Mock<ITemperatureRetriever>();

            var temperature = new Temperature
            {
                DegreesInCelsium = -5.67,
                Timestamp = DateTime.UtcNow
            };

            temperatureRetriever
                .Setup(x => x.GetTemperatureAsync())
                .ReturnsAsync(temperature);

            var temperatureService = new TemperatureService(temperatureRepository.Object, temperatureRetriever.Object);

            await temperatureService.StoreTemperatureAsync();

            temperatureRepository.Verify(x => x.StoreTemperatureAsync(temperature), Times.Once);
        }
    }
}
