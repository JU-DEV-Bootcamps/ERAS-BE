using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Utils;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Eras.Application.Services
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IAnswerService, AnswerService>();
            services.AddScoped<PollOrchestratorService, PollOrchestratorService>();

            return services;
        }
    }
}
