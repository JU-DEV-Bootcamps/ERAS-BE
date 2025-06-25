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
                    Description = "",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                }
                );

                Options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Scheme"
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
