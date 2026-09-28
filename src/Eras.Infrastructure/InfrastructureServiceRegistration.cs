using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Models;
using Eras.Application.Models.Response.HeatMap;
using Eras.Application.Services;
using Eras.Application.Utils;
using Eras.Domain.Common;
using Eras.Infrastructure.Authentication;
using Eras.Infrastructure.Authorization;
using Eras.Infrastructure.Cryptography;
using Eras.Infrastructure.External.CosmicLatteClient;
using Eras.Infrastructure.External.KeycloakClient;
using Eras.Infrastructure.FileStorage;
using Eras.Infrastructure.Persistence.PostgreSQL.Jobs;

using Keycloak.AuthServices.Authorization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Eras.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection Services,
            IConfiguration Configuration)
        {
            Services.AddScoped<IKeycloakAuthService<TokenResponse>, KeycloakAuthService>();
            Services.AddHttpContextAccessor();
            Services.AddScoped<ICurrentUserService, CurrentUserService>();
            Services.AddScoped<ICosmicLatteAPIService, CosmicLatteAPIService>();
            Services.AddScoped<IApiKeyEncryptor, AesApiKeyEncryptor>();
            Services.AddScoped<IAnswerRiskValidator, AnswerRiskValidator>();
            Services.AddHostedService<EvaluationStatusSyncJob>();
            Services.AddSingleton<IImportJobQueue, BackgroundProcessing.ImportJobQueue>();
            Services.AddHostedService<BackgroundProcessing.ImportQueueBackgroundService>();

            Services.Configure<FileStorageSettings>(Configuration.GetSection("FileStorage"));

            string basePath = Configuration["FileStorage:BasePath"] ?? "/app/uploads";
            
            Services.Configure<FileStorageSettings>(Configuration.GetSection("FileStorage"));

            Services.AddSingleton<IFileEncryptionService, AesFileEncryptionService>();

            Services.AddSingleton<IFileStorageService>(sp =>
                new LocalFileStorageService(
                    Configuration["FileStorage:BasePath"] ?? "/app/uploads",
                    sp.GetRequiredService<IFileEncryptionService>(),
                    sp.GetRequiredService<ILogger<LocalFileStorageService>>()
                ));
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
                    var audience = Configuration["Keycloak:Audience"];

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

            // "public-client" is the OAuth client real end users authenticate as (the FE's
            // Keycloak client). Bearer tokens hitting this API carry their roles under
            // resource_access[public-client], regardless of which client issued them,
            // because both clients have fullScopeAllowed enabled in the realm.
            string rolesResource = Configuration["Keycloak:RolesResource"] ?? "public-client";

            Services.AddKeycloakAuthorization(Options =>
            {
                Options.EnableRolesMapping = RolesClaimTransformationSource.ResourceAccess;
                Options.RolesResource = rolesResource;
            });

            Services.AddAuthorization(Options => ErasPolicies.Configure(Options, rolesResource));
        }
    }
}
