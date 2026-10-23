using System.Net;

using Eras.Infrastructure.External.CosmicLatteClient;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Moq;

using Xunit;

namespace Eras.Infrastructure.Tests.External.CosmicLatteClient
{
    public class CosmicLatteHealthCheckTests
    {
        private const string BaseUrl = "http://fakeurl.com/";
        private const string ExpectedRequestUrl = BaseUrl + "evaluations?$filter=contains(name,' ')";

        private static Mock<IConfiguration> CreateConfig(string? apiKey = "fake-api-key", string? baseUrl = BaseUrl)
        {
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c.GetSection("CosmicLatte:ApiKey").Value).Returns(apiKey);
            configuration.Setup(c => c.GetSection("CosmicLatte:BaseUrl").Value).Returns(baseUrl);
            return configuration;
        }

        private static HttpClient CreateHttpClient(HttpResponseMessage response)
        {
            var responses = new Dictionary<string, HttpResponseMessage>
            {
                { ExpectedRequestUrl, response }
            };
            var handler = new MockHttpMessageHandler(responses);
            return new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
        }

        // ---------- Constructor ----------

        [Fact]
        public void Constructor_MissingApiKey_ThrowsException()
        {
            // Arrange
            var configuration = CreateConfig(apiKey: null);
            var httpClient = new HttpClient();

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => new CosmicLatteHealthCheck(configuration.Object, httpClient));
            Assert.Equal("Cosmic latte api key not found", ex.Message);
        }

        [Fact]
        public void Constructor_MissingBaseUrl_ThrowsException()
        {
            // Arrange
            var configuration = CreateConfig(baseUrl: null);
            var httpClient = new HttpClient();

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => new CosmicLatteHealthCheck(configuration.Object, httpClient));
            Assert.Equal("Cosmic latte Url not found", ex.Message);
        }

        // ---------- CheckHealthAsync ----------

        [Fact]
        public async Task CheckHealthAsync_SuccessResponse_ReturnsHealthy()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            };
            var configuration = CreateConfig();
            var httpClient = CreateHttpClient(response);
            var healthCheck = new CosmicLatteHealthCheck(configuration.Object, httpClient);

            // Act
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert
            Assert.Equal(HealthStatus.Healthy, result.Status);
            Assert.Equal("Cosmic Latte service is available", result.Description);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.ContainsKey("ResponseTime"));
            Assert.Equal((int)HttpStatusCode.OK, (int)(HttpStatusCode)result.Data["StatusCode"]);
        }

        [Fact]
        public async Task CheckHealthAsync_UnsuccessfulResponse_ReturnsUnhealthy()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("error")
            };
            var configuration = CreateConfig();
            var httpClient = CreateHttpClient(response);
            var healthCheck = new CosmicLatteHealthCheck(configuration.Object, httpClient);

            // Act
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert
            Assert.Equal(HealthStatus.Unhealthy, result.Status);
            Assert.Equal("Cosmic Latte service returned an unsuccessful status code", result.Description);
        }

        [Fact]
        public async Task CheckHealthAsync_HttpClientThrows_ThrowsWrappedException()
        {
            // Arrange: URL que no coincide con ninguna respuesta configurada -> el handler devuelve 404,
            // así que para forzar una excepción real usamos un handler que explota directamente.
            var configuration = CreateConfig();
            var throwingHandler = new ThrowingHttpMessageHandler();
            var httpClient = new HttpClient(throwingHandler) { BaseAddress = new Uri(BaseUrl) };
            var healthCheck = new CosmicLatteHealthCheck(configuration.Object, httpClient);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => healthCheck.CheckHealthAsync(new HealthCheckContext()));
            Assert.Equal("There was an error with the request", ex.Message);
        }

        private class ThrowingHttpMessageHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => throw new HttpRequestException("network failure");
        }
    }
}