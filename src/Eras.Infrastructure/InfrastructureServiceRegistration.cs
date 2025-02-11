using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Services;
using Eras.Infrastructure.External.CosmicLatteClient;
using Eras.Infrastructure.External.KeycloakClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Eras.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IKeycloakAuthService<TokenResponse>, KeycloakAuthService>();
            services.AddScoped<ICosmicLatteAPIService, CosmicLatteAPIService>();
            
            AddAuthentication(services);

            return services;
        }

        private static void GetKeycloakConfiguration(
            out string keycloakBaseUrl,
            out string keycloakRealm)
        {
            keycloakBaseUrl = Environment.GetEnvironmentVariable("KEYCLOAK_BASE_URL");
            keycloakRealm = Environment.GetEnvironmentVariable("KEYCLOAK_REALM");
        }

         private static void AddAuthentication(IServiceCollection services)
        {
            GetKeycloakConfiguration(
                out string keycloakBaseUrl,
                out string keycloakRealm);

            services.AddAuthorization();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"{keycloakBaseUrl}/realms/{keycloakRealm}",

                    ValidateAudience = true,
                    ValidAudience = "account",

                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false,

                    IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                    {
                        var client = new HttpClient();
                        var keyUri = $"{parameters.ValidIssuer}/protocol/openid-connect/certs";
                        var response = client.GetAsync(keyUri).Result;
                        var keys = new JsonWebKeySet(response.Content.ReadAsStringAsync().Result);

                        return keys.GetSigningKeys();
                    }
                };

                options.RequireHttpsMetadata = false; // Only in develop environment
                options.SaveToken = true;
            });
        }
    }
}