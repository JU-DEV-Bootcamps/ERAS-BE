using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;

namespace Eras.Api
{
    [ExcludeFromCodeCoverage]
    public static class ApiServiceRegistration
    {
        public static IServiceCollection AddApiServices(
            this IServiceCollection Services,
            IConfiguration Configuration)
        {
            Services.AddHttpClient();
            Services.AddControllers();
            Services.AddEndpointsApiExplorer();
            Services.AddSwaggerGen(Options =>
            {
                Options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer {token}' here",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }
                );

                Options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                        }
                    });
            });

            AddCors(Services, Configuration);

            return Services;
        }

        private static void AddCors(
            IServiceCollection Services,
            IConfiguration Configuration)
        {
            Services.AddCors(O =>
            {
                O.AddPolicy("CORSPolicy", Policy =>
                {
                    string allowedHosts = Configuration["AllowedHosts"] ?? "*";
                    Policy.WithOrigins(allowedHosts)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                });
            });
        }
    }
}
