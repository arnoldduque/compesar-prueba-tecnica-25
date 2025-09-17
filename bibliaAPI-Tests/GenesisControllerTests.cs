using bibliaAPI.Controllers;
using bibliaAPI.Model.Input;
using bibliaAPI.Model.Output;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;

namespace bibliaAPI_Tests
{
    public class GenesisControllerTests
    {
        private GenesisController CreateControllerWithMocks(HttpResponseMessage httpResponse)
        {
            // Mock ILogger
            var loggerMock = new Mock<ILogger<GenesisController>>();

            // Mock IHttpClientFactory
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            var httpClient = new HttpClient(handlerMock.Object);
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Mock IConfiguration
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["ExternalApi:BibleAPI:GenesisUrl"]).Returns("http://fakeapi");

            // In-memory DbContext
            var options = new DbContextOptionsBuilder<BibleContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            var dbContext = new BibleContext(options);

            return new GenesisController(
                loggerMock.Object,
                httpClientFactoryMock.Object,
                configMock.Object,
                dbContext
            );
        }

        [Fact]
        public async Task GetAsync_ReturnsOkResult_WithConsulta()
        {
            // Arrange
            var fakeBook = new BibleBook
            {
                verses =
                [
                    new Verse { verse = 1, text = "In the beginning..." }
                ]
            };
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(fakeBook))
            };
            var controller = CreateControllerWithMocks(httpResponse);

            // Act
            var result = await controller.GetAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var consulta = Assert.IsType<Consulta>(okResult.Value);
            Assert.Single(consulta!.Genesis!);
            Assert.Single(consulta!.Genesis![0].Verses!);
            Assert.Equal("In the beginning...", consulta!.Genesis![0].Verses![0].Text);
        }
    }
}