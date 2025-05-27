using System.Diagnostics.CodeAnalysis;

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
            Services.AddSwaggerGen();

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