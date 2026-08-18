using System.Net;
using System.Text.Json;

using Eras.Infrastructure.External.KeycloakClient;

using Microsoft.Extensions.Configuration;

using Moq;

using Xunit;

namespace Eras.Infrastructure.Tests.External.KeycloakClient
{
    public class KeycloakAuthServiceTests
    {
        private const string BaseUrl = "http://fakeurl.com";
        private const string Realm = "eras-realm";
        private const string TokenEndpoint = BaseUrl + "/realms/" + Realm + "/protocol/openid-connect/token";

        private Mock<IConfiguration> CreateConfig()
        {
            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Keycloak:BaseUrl"]).Returns(BaseUrl);
            config.Setup(c => c["Keycloak:Realm"]).Returns(Realm);
            config.Setup(c => c["Keycloak:ClientId"]).Returns("fake-client-id");
            config.Setup(c => c["Keycloak:ClientSecret"]).Returns("fake-client-secret");
            return config;
        }

        private KeycloakAuthService CreateService(HttpResponseMessage response, Mock<IConfiguration>? config = null)
        {
            var responses = new Dictionary<string, HttpResponseMessage> { { TokenEndpoint, response } };
            var handler = new MockHttpMessageHandler(responses);
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };

            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
            factoryMock.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient);

            return new KeycloakAuthService((config ?? CreateConfig()).Object, factoryMock.Object);
        }

        [Fact]
        public async Task LoginAsync_SuccessResponse_ReturnsDeserializedToken()
        {
            // Arrange
            var tokenJson = JsonSerializer.Serialize(new { access_token = "abc123", expires_in = 300 });
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(tokenJson) };
            var service = CreateService(response);

            // Act
            var result = await service.LoginAsync("user@test.com", "password123");

            // Assert
            Assert.Equal("abc123", result.AccessToken);
            Assert.Equal(300, result.ExpiresIn);
        }

        [Fact]
        public async Task LoginAsync_UnsuccessfulResponse_ThrowsExceptionWithStatusCode()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("{\"error\":\"invalid_grant\"}")
            };
            var service = CreateService(response);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => service.LoginAsync("user@test.com", "wrongpassword"));
            Assert.Contains("Authentication failed", ex.Message);
            Assert.Contains("Unauthorized", ex.Message);
        }

        [Fact]
        public async Task LoginAsync_BuildsCorrectTokenEndpointFromConfig()
        {
            // Arrange: si la URL no matchea exactamente lo que arma el servicio,
            // MockHttpMessageHandler devuelve 404 por default y el test de abajo fallaría
            // con un mensaje distinto al esperado -- esto confirma que la URL se arma bien.
            var tokenJson = JsonSerializer.Serialize(new { access_token = "xyz", expires_in = 60 });
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(tokenJson) };
            var service = CreateService(response);

            // Act
            var result = await service.LoginAsync("user@test.com", "password123");

            // Assert
            Assert.Equal("xyz", result.AccessToken);
        }

        [Fact]
        public async Task LoginAsync_MissingClientCredentials_StillSendsRequestWithNullValues()
        {
            // Arrange: ClientId/ClientSecret null -> el "!" en el código los fuerza,
            // pero FormUrlEncodedContent no debería explotar con valores null convertidos.
            // Si el compilador null-forgiving oculta un NullReferenceException real en runtime,
            // este test lo va a mostrar.
            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Keycloak:BaseUrl"]).Returns(BaseUrl);
            config.Setup(c => c["Keycloak:Realm"]).Returns(Realm);
            config.Setup(c => c["Keycloak:ClientId"]).Returns((string?)null);
            config.Setup(c => c["Keycloak:ClientSecret"]).Returns((string?)null);

            var tokenJson = JsonSerializer.Serialize(new { access_token = "abc", expires_in = 60 });
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(tokenJson) };
            var service = CreateService(response, config);

            // Act & Assert: no debería tirar excepción por los nulls al armar el FormUrlEncodedContent
            var result = await service.LoginAsync("user@test.com", "password123");
            Assert.Equal("abc", result.AccessToken);
        }
    }
}