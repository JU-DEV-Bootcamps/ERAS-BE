using Eras.Application.Contracts.Persistence;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eras.Infrastructure.Persistence.PostgreSQL
{
    public static class PersistenceServiceRegistration
    {
        private static string GetConnectionString(IConfiguration Configuration)
        {
            var postgresHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? Configuration["Postgres:Host"];
            var postgresPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? Configuration["Postgres:Port"];
            var postgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? Configuration["Postgres:Username"];
            var postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? Configuration["Postgres:Password"];
            var postgresDb = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? Configuration["Postgres:Database"];

            return $"Host={postgresHost};Port={postgresPort};Username={postgresUser};Password={postgresPassword};Database={postgresDb}";
        }

        public static void AddDbContextPostgreSql(IServiceCollection Services, IConfiguration Configuration)
        {
            var connectionString = GetConnectionString(Configuration);

            Services.AddDbContext<AppDbContext>(Options =>
            {
                Options.UseNpgsql(connectionString);
                Options.ConfigureWarnings(W => W.Ignore(RelationalEventId.PendingModelChangesWarning));
            });
        }

        public static IServiceCollection AddPersistenceServices(this IServiceCollection Services, IConfiguration Configuration)
        {
            AddDbContextPostgreSql(Services, Configuration);
            Services.AddScoped<IAnswerRepository, AnswerRepository>();
            Services.AddScoped<ICohortRepository, CohortRepository>();
            Services.AddScoped<IComponentRepository, ComponentRepository>();
            Services.AddScoped<IPollInstanceRepository, PollInstanceRepository>();
            Services.AddScoped<IPollRepository, PollRepository>();
            Services.AddScoped<IStudentDetailRepository, StudentDetailRepository>();
            Services.AddScoped<IStudentRepository, StudentRepository>();
            Services.AddScoped<IVariableRepository, VariableRepository>();
            Services.AddScoped<IPollVariableRepository, PollVariableRepository>();
            Services.AddScoped<IStudentCohortRepository, StudentCohortRepository>();
            Services.AddScoped<IPollCohortRepository, PollCohortRepository>();
            Services.AddScoped<IHeatMapRepository, HeatMapRespository>();
            Services.AddScoped<IEvaluationRepository, EvaluationRepository>();
            Services.AddScoped<IEvaluationPollRepository, EvaluationPollRepository>();
            Services.AddScoped<IStudentPollsRepository, StudentPollsRepository>();
            Services.AddScoped<IComponentsAvgRepository, ComponentsAvgRepository>();
            Services.AddScoped<IStudentAnswersRepository, StudentAnswersRepository>();
            Services.AddScoped<IConfigurationsRepository, ConfigurationsRepository>();
            Services.AddScoped<IServiceProvidersRepository, ServiceProvidersRepository>();
            Services.AddScoped<IRemissionRepository, RemissionRepository>();

            return Services;
        }
    }
}
