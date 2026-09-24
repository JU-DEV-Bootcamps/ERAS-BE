using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Mappers;
using Eras.Domain.Common;
using Eras.Domain.Entities.UserManagement;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Eras.Infrastructure.External.KeycloakClient
{
    public class KeycloakAuthService : IKeycloakAuthService<TokenResponse>
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KeycloakAuthService> _logger;
        private readonly IErasUsersRepository _erasUserRepository;
        private readonly JwtSecurityTokenHandler _jwtHandler = new ();

        public KeycloakAuthService(
            IConfiguration Configuration,
            IHttpClientFactory HttpClientFactory,
            ILogger<KeycloakAuthService> Logger,
            IErasUsersRepository ErasUserRepository)
        {
            _httpClient = HttpClientFactory.CreateClient();
            _configuration = Configuration;
            _logger = Logger;
            _erasUserRepository = ErasUserRepository;
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

                ErasUserDTO? existingUser = await _erasUserRepository.GetErasUserBySubAsync(keycloakClaims.Sub)
                    ?? await _erasUserRepository.GetErasUserByEmailAsync(keycloakClaims.Email);

                if (existingUser is null)
                {
                    var erasUser = new ErasUserDTO
                    {
                        Email = keycloakClaims.Email,
                        FirstName = keycloakClaims.GivenName,
                        LastName = keycloakClaims.FamilyName,
                        Sub = keycloakClaims.Sub,
                        Role = GetUserRole(keycloakClaims.ResourceAccess, ClientId),
                        Audit = new AuditInfo
                        {
                            CreatedAt = DateTime.Now,
                            CreatedBy = "System",
                            ModifiedBy = "System",
                            ModifiedAt = DateTime.Now,
                        },
                        IsSynced = true,
                    };

                    ErasUser persisted = await _erasUserRepository.AddAsync(erasUser.ToDomain());

                    _logger.LogInformation($"ERAS User ${persisted.Sub} has been created.");
                } else if (existingUser.IsSynced == false)
                {
                    existingUser.IsSynced = true;
                    existingUser.Email = keycloakClaims.Email;
                    existingUser.FirstName = keycloakClaims.GivenName;
                    existingUser.LastName = keycloakClaims.FamilyName;
                    existingUser.Sub = keycloakClaims.Sub;
                    existingUser.Role = GetUserRole(keycloakClaims.ResourceAccess, ClientId);
                    existingUser.Audit.ModifiedAt = DateTime.Now;
                    existingUser.Audit.ModifiedBy = "System";

                    ErasUser updated = await _erasUserRepository.UpdateAsync(existingUser.ToDomain());

                    _logger.LogInformation($"ERAS User ${updated.Sub} has been updated.");
                }
            } catch(Exception Ex)
            {
                _logger.LogError("ERAS User registration failed. User was not created in the database.");
                _logger.LogError(Ex.Message);
            }
            
        }

        private string GetUserRole(Dictionary<string, Resource> ResourceAccess, string? ClientId)
        {
            var defaultRole = ErasRole.Guest.Label;

            if (string.IsNullOrEmpty(ClientId))
                return defaultRole;

            ResourceAccess.TryGetValue(ClientId, out Resource? resource);
            
            if (resource is null)
                return defaultRole;
            
            var userRoles = resource.roles;

            if(userRoles.Length == 0)
                return defaultRole;

            if (userRoles.Contains(ErasRole.Administrator.Label))
                return ErasRole.Administrator.Label;
            
            userRoles = userRoles.Where(Role => ErasRole.ListLabels().Contains(Role)).ToArray();
            if (userRoles.Length > 0)
                return userRoles[0];
            else
                return defaultRole;
        }
    }
}
