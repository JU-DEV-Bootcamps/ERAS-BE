using System.Diagnostics.CodeAnalysis;

namespace Eras.Api
{
    [ExcludeFromCodeCoverage]
    public static class ApiServiceRegistration
    {
        public static IServiceCollection AddApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            AddCors(services, configuration);

            return services;
        }

        private static void AddCors(
            IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddCors(o =>
            {
                o.AddPolicy("CORSPolicy", policy =>
                {
                    string allowedHosts = configuration["AllowedHosts"] ?? "*";
                    policy.WithOrigins(allowedHosts)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                });
            });
        }
    }
}