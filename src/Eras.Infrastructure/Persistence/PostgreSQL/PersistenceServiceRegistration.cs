using Eras.Domain.Repositories;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Eras.Infrastructure.Persistence.PostgreSQL
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IAnswerRepository, AnswerRepository>();
            services.AddScoped<IComponentRepository, ComponentRepository>();
            services.AddScoped<IPollRepository, PollRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            
            return services;
        }
    }
}