using Eras.Application.Contracts.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Eras.Application.Services
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {
            Services.AddMediatR(Cfg => Cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

            Services.AddScoped<IStudentService, StudentService>();
            Services.AddScoped<IPollService, PollService>();
            Services.AddScoped<IAnswerService, AnswerService>();
            Services.AddScoped<PollOrchestratorService, PollOrchestratorService>();
            
            return Services;
        }
    }
}
