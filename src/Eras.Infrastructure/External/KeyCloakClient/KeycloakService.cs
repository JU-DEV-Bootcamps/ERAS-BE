using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Features.ErasUsers;
using Eras.Domain.Entities.UserManagement;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Eras.Infrastructure.External.KeycloakClient
{
    public class KeycloakAuthService : IKeycloakAuthService<TokenResponse>
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KeycloakAuthService> _logger;
        private readonly IMediator _mediator;
        private readonly JwtSecurityTokenHandler _jwtHandler = new ();

        public KeycloakAuthService(
            IConfiguration Configuration,
            IHttpClientFactory HttpClientFactory,
            ILogger<KeycloakAuthService> Logger,
            IMediator Mediator)
        {
            _httpClient = HttpClientFactory.CreateClient();
            _configuration = Configuration;
            _logger = Logger;
            _mediator = Mediator;
        }

        public async Task<TokenResponse> LoginAsync(string Username, string Password)
        {
            var baseUrl = _configuration["Keycloak:BaseUrl"];
            var realm = _configuration["Keycloak:Realm"];
            var clientId = _configuration["Keycloak:ClientId"];
            var clientSecret = _configuration["Keycloak:ClientSecret"];
            var tokenEndpoint = $"{baseUrl}/realms/{realm}/protocol/openid-connect/token";

            var content = new FormUrlEncodedContent(new[]{
                new KeyValuePair<string,string>("grant_type", "password"),
                new KeyValuePair<string,string>("client_id", clientId!),
                new KeyValuePair<string,string>("client_secret", clientSecret!),
                new KeyValuePair<string, string>("username", Username),
                new KeyValuePair<string, string>("password", Password),
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                TokenResponse token = JsonSerializer.Deserialize<TokenResponse>(responseContent)
                    ?? throw new Exception("Authentication failed: Token could not be deserialized.");

                // Handle user creation or update for DB table eras_users
                await RegisterErasUser(token, clientId);

                return token;
            }

            throw new Exception($"Authentication failed {response.StatusCode}: \n{response.Content}");
        }

        private KeycloakClaims GetKeycloakClaims(TokenResponse token)
        {
            JwtSecurityToken deserializedToken = _jwtHandler.ReadJwtToken(token.AccessToken);

            var userSub = deserializedToken.Claims.First(C => C.Type == "sub").Value;
            var userEmail = deserializedToken.Claims.First(C => C.Type == "email").Value;
            var userGivenName = deserializedToken.Claims.First(C => C.Type == "given_name").Value;
            var userFamilyName = deserializedToken.Claims.First(C => C.Type == "family_name").Value;
            Dictionary<string, Resource>? userResourceAccess =
                JsonSerializer.Deserialize<Dictionary<string, Resource>>(
                    deserializedToken.Claims.First(C => C.Type == "resource_access").Value
                );

            return new KeycloakClaims
            {
                Sub = userSub,
                Email = userEmail,
                GivenName = userGivenName,
                FamilyName = userFamilyName,
                ResourceAccess = userResourceAccess!
            };
        }

        protected async Task RegisterErasUser(TokenResponse Token, string? ClientId)
        {
            try
            {
                KeycloakClaims keycloakClaims = GetKeycloakClaims(Token);

                string[] roles = !string.IsNullOrEmpty(ClientId)
                    && keycloakClaims.ResourceAccess.TryGetValue(ClientId, out Resource? resource)
                        ? resource.roles
                        : [];

                await _mediator.Send(new SyncErasUserCommand(
                    keycloakClaims.Sub,
                    keycloakClaims.Email,
                    keycloakClaims.GivenName,
                    keycloakClaims.FamilyName,
                    ErasRole.Resolve(roles)
                ));
            } catch(Exception Ex)
            {
                _logger.LogError("ERAS User registration failed. User was not created in the database.");
                _logger.LogError(Ex.Message);
            }
        }
    }
}
