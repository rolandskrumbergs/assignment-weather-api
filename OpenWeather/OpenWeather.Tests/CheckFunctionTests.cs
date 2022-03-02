using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenWeather.API;
using OpenWeather.Domain;
using System;
using System.Threading.Tasks;

namespace OpenWeather.Tests
{
    [TestClass]
    public class CheckFunctionTests
    {
        [TestMethod]
        public async Task Empty_Query_Should_Return_BadRequest()
        {
            var query = new Mock<IQueryCollection>();
            var request = new Mock<HttpRequest>();
            request
                .SetupGet(x => x.Query)
                .Returns(query.Object);

            var logger = new Mock<ILogger>();

            var temperatureService = new Mock<ITemperatureService>();
            temperatureService
                .Setup(x => x.GetTemperatureAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException());

            var function = new WeatherAPI(temperatureService.Object);

            var result = await function.CheckAsync(request.Object, logger.Object);

            var expectedResult = new BadRequestObjectResult("Invalid date or time!");

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task Error_Should_Return_InternalServerError()
        {
            var query = new Mock<IQueryCollection>();
            var request = new Mock<HttpRequest>();
            request
                .SetupGet(x => x.Query)
                .Returns(query.Object);

            var logger = new Mock<ILogger>();

            var temperatureService = new Mock<ITemperatureService>();
            temperatureService
                .Setup(x => x.GetTemperatureAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var function = new WeatherAPI(temperatureService.Object);

            var result = await function.CheckAsync(request.Object, logger.Object);

            var expectedResult = new StatusCodeResult(StatusCodes.Status500InternalServerError);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task Valid_Request_Should_Return_Result()
        {
            var query = new Mock<IQueryCollection>();
            var request = new Mock<HttpRequest>();
            request
                .SetupGet(x => x.Query)
                .Returns(query.Object);

            var logger = new Mock<ILogger>();

            var temperature = new Temperature
            {
                DegreesInCelsium = 10,
                Timestamp = DateTime.UtcNow
            };

            var temperatureService = new Mock<ITemperatureService>();
            temperatureService
                .Setup(x => x.GetTemperatureAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(temperature);

            var function = new WeatherAPI(temperatureService.Object);

            var result = await function.CheckAsync(request.Object, logger.Object);

            var expectedResult = new OkObjectResult(temperature);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task Valid_Request_With_Non_Existing_Record_Returns_NotFoundObjectResult()
        {
            var query = new Mock<IQueryCollection>();
            var request = new Mock<HttpRequest>();
            request
                .SetupGet(x => x.Query)
                .Returns(query.Object);

            var logger = new Mock<ILogger>();

            var temperatureService = new Mock<ITemperatureService>();
            temperatureService
                .Setup(x => x.GetTemperatureAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Temperature)default);

            var function = new WeatherAPI(temperatureService.Object);

            var result = await function.CheckAsync(request.Object, logger.Object);

            var expectedResult = new NotFoundResult();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task StoreTemperature_Does_Not_Throw_Exception()
        {
            var timer = new TimerInfo(null, null);

            var logger = new Mock<ILogger>();

            var temperatureService = new Mock<ITemperatureService>();

            var function = new WeatherAPI(temperatureService.Object);

            await function.GatherAsync(timer, logger.Object);
        }
    }
}