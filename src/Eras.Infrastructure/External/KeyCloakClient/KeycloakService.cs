using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using Eras.Application.Contracts.Infrastructure;

using Microsoft.Extensions.Configuration;


namespace Eras.Infrastructure.External.KeycloakClient
{
    [ExcludeFromCodeCoverage]
    public class KeycloakAuthService : IKeycloakAuthService<TokenResponse>
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public KeycloakAuthService(IConfiguration Configuration, IHttpClientFactory HttpClientFactory)
        {
            _httpClient = HttpClientFactory.CreateClient();
            _configuration = Configuration;
        }

        public async Task<TokenResponse> LoginAsync(string Username, string Password)
        {
            var baseUrl = _configuration["Keycloak:BaseUrl"];
            var realm = _configuration["Keycloak:Realm"];
            var clientId = _configuration["Keycloak:ClientId"];
            var clientSecret = _configuration["Keycloak:ClientSecret"];
            var tokenEndpoint = $"{baseUrl}/realms/{realm}/protocol/openid-connect/token";


            var content = new FormUrlEncodedContent(new[]{
                new KeyValuePair<string,string>("grant_type", "client_credentials"),
                new KeyValuePair<string,string>("client_id", clientId!),
                new KeyValuePair<string,string>("client_secret", clientSecret!),
                new KeyValuePair<string, string>("username", Username),
                new KeyValuePair<string, string>("password", Password),
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<TokenResponse>(responseContent)!;
            }

            throw new Exception($"Authentication failed {response.StatusCode}: \n{response.Content}");
        }
    }
}
