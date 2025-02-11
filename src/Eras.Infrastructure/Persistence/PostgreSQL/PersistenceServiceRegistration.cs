using Eras.Application.Contracts.Persistence;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Eras.Infrastructure.Persistence.PostgreSQL
{
    public static class PersistenceServiceRegistration
    {
        private static string GetConnectionString()
        {
            var postgresHost = Environment.GetEnvironmentVariable("POSTGRES_HOST");
            var postgresPort = Environment.GetEnvironmentVariable("POSTGRES_PORT");
            var postgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
            var postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            var postgresDb = Environment.GetEnvironmentVariable("POSTGRES_DB");

            return $"Host={postgresHost};Port={postgresPort};Username={postgresUser};Password={postgresPassword};Database={postgresDb}";
        }

        public static void AddDbContextPostgreSql(IServiceCollection services)
        {
            var connectionString = GetConnectionString();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
            });
        }

        public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
        {
            AddDbContextPostgreSql(services);
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IAnswerRepository, AnswerRepository>();
            services.AddScoped<IComponentRepository, ComponentRepository>();
            services.AddScoped<IPollRepository, PollRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            
            return services;
        }
    }
}