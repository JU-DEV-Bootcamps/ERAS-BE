using System.Reflection;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.DTOs.RemissionManagement;
using Eras.Application.Mappers;
using Eras.Application.Mappers.RemissionManagement;
using Eras.Application.Models.Response.HeatMap;
using Eras.Domain.Entities.RemissionsManagement;
using Eras.Domain.Entities.RemissionsManagement.Validators;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

namespace Eras.Application.Services
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {
            Services.AddMediatR(Cfg => Cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            Services.AddScoped<IStudentService, StudentService>();
            Services.AddScoped<IPollService, PollService>();
            Services.AddScoped<IAnswerService, AnswerService>();
            Services.AddScoped<PollOrchestratorService, PollOrchestratorService>();
            Services.ConfigureMappers();
            return Services;
        }


        public static IServiceCollection ConfigureMappers(this IServiceCollection services)
        {

            services.AddScoped<IMapper<StudentProfileDto, StudentProfile>, StudentProfileMapper>();
            services.AddScoped<IMapper<StudentProfile, StudentProfileDto>, StudentProfileToDtoMapper>();

            services.AddScoped<IMapper<InterventionPlanDto, InterventionPlan>, InterventionPlanMapper>();
            services.AddScoped<IMapper<InterventionPlan, InterventionPlanDto>, InterventionPlanToDtoMapper>();

            services.AddScoped<IMapper<IndividualInterventionDto, IndividualIntervention>, IndividualInterventionMapper>();
            services.AddScoped<IMapper<IndividualIntervention, IndividualInterventionDto>, IndividualInterventionToDtoMapper>();

            services.AddScoped<IMapper<GroupInterventionDto, GroupIntervention>, GroupInterventionMapper>();
            services.AddScoped<IMapper<GroupIntervention, GroupInterventionDto>, GroupInterventionToDtoMapper>();

            services.AddScoped<IMapper<RemissionDto, Remission>, RemissionMapper>();
            services.AddScoped<IMapper<Remission, RemissionDto>, RemissionToDtoMapper>();

            services.AddScoped<IValidator<StudentProfile>, StudentProfileValidator>();
            services.AddScoped<IValidator<InterventionPlan>, InterventionPlanValidator>();
            services.AddScoped<IValidator<Remission>, RemissionValidator>();

            return services;
        }
    }
}
