using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Services;
using Eras.Domain.Common;
using Eras.Infrastructure.Cryptography;
using Eras.Infrastructure.External.CosmicLatteClient;
using Eras.Infrastructure.External.KeycloakClient;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Eras.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection Services,
            IConfiguration Configuration)
        {
            Services.AddScoped<IKeycloakAuthService<TokenResponse>, KeycloakAuthService>();
            Services.AddScoped<ICosmicLatteAPIService, CosmicLatteAPIService>();
            Services.AddScoped<IApiKeyEncryptor, AesApiKeyEncryptor>();

            AddAuthentication(Services, Configuration);

            return Services;
        }

        private static void GetKeycloakConfiguration(
            IConfiguration Configuration,
            out string? KeycloakBaseUrl,
            out string? KeycloakRealm)
        {
            KeycloakBaseUrl = Environment.GetEnvironmentVariable("KEYCLOAK_BASE_URL") ?? Configuration["Keycloak:BaseUrl"];
            KeycloakRealm = Environment.GetEnvironmentVariable("KEYCLOAK_REALM") ?? Configuration["Keycloak:Realm"];
        }

        private static void AddAuthentication(IServiceCollection Services, IConfiguration Configuration)
        {
            GetKeycloakConfiguration(
                Configuration,
                out string? keycloakBaseUrl,
                out string? keycloakRealm);

            Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(Options =>
                {
                    var audience = Configuration["Keycloak:ClientId"];

                    Options.Authority = $"{keycloakBaseUrl}/realms/{keycloakRealm}";

                    Options.Audience = audience;

                    Options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = $"{keycloakBaseUrl}/realms/{keycloakRealm}",

                        ValidateAudience = true,
                        ValidAudience = audience,

                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,

                        IssuerSigningKeyResolver = (Token, SecurityToken, Kid, Parameters) =>
                        {
                            var client = new HttpClient();
                            var keyUri = $"{Parameters.ValidIssuer}/protocol/openid-connect/certs";
                            var response = client.GetAsync(keyUri).Result;
                            var keys = new JsonWebKeySet(response.Content.ReadAsStringAsync().Result);

                            return keys.GetSigningKeys();
                        }
                    };

                    Options.RequireHttpsMetadata = false; // Only in develop environment

                });
            Services.AddAuthorization();

        }
    }
}
