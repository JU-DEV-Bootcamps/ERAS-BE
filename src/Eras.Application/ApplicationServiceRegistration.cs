using Eras.Application.Contracts.Infrastructure;
using Eras.Application.DTOs.RemissionManagement;
using Eras.Application.Mappers.RemissionManagement;
using Eras.Domain.Entities.RemissionsManagement;

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
            Services.ConfigureMappers();

            return Services;
        }
        private static IServiceCollection ConfigureMappers(this IServiceCollection services)
        {
            services.AddScoped<IMapper<StudentProfileDto, StudentProfile>, StudentProfileMapper>();

            services.AddScoped<IMapper<InterventionPlanDto, InterventionPlan>, InterventionPlanMapper>();
            services.AddScoped<IMapper<IndividualInterventionDto, IndividualIntervention>, IndividualInterventionMapper>();
            services.AddScoped<IMapper<GroupInterventionDto, GroupIntervention>, GroupInterventionMapper>();
            services.AddScoped<IMapper<RemissionDto, Remission>, RemissionMapper>();

            return services;
        }

    }
}
