using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Eras.Infrastructure.External.KeycloakClient
{
    public class KeycloakAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public KeycloakAuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        public async Task<TokenResponse> LoginAsync(string username, string password)
        {
            var baseUrl = _configuration["Keycloak:BaseUrl"];
            var realm = _configuration["Keycloak:Realm"];
            var clientId =  _configuration["Keycloak:ClientId"];
            var clientSecret = _configuration["Keycloak:ClientSecret"];
            var tokenEndpoint = $"{baseUrl}/realms/{realm}/protocol/openid-connect/token";

            var requestBody = new Dictionary<string, string>{
                { "grant_type", "password" },
                { "client_id",  clientId},
                { "client_secret", clientSecret },
                { "username", username },
                { "password", password }
            };

            var content = new FormUrlEncodedContent(requestBody);

            var response = await _httpClient.PostAsync(tokenEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResult = JsonSerializer.Deserialize<TokenResponse>(responseContent);

                return tokenResult!;
            }

            throw new Exception($"Authentication failed {response.StatusCode}: \n{response.Content}");
        }
    }
}
