using Eras.Application.Contracts.Persistence;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Eras.Infrastructure.Persistence.PostgreSQL
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();
            services.AddScoped<IAnswerRepository, AnswerRepository>();
            services.AddScoped<ICohortRepository, CohortRepository>();
            services.AddScoped<IComponentRepository, ComponentRepository>();
            services.AddScoped<IPollInstanceRepository, PollInstanceRepository>();
            services.AddScoped<IPollRepository, PollRepository>();
            services.AddScoped<IStudentDetailRepository, StudentDetailRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IVariableRepository, VariableRepository>();

            return services;
        }
    }
}