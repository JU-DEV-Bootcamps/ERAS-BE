using System.Reflection;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Application.Models.Response.HeatMap;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement.Validators;

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
            Services.AddScoped<EvaluationStatusUpdater>();
            Services.ConfigureMappers();
            return Services;
        }


        public static IServiceCollection ConfigureMappers(this IServiceCollection services)
        {

            services.AddScoped<IMapper<StudentProfileDto, Student>, StudentProfileMapper>();
            services.AddScoped<IMapper<Student, StudentProfileDto>, StudentProfileToDtoMapper>();

            services.AddScoped<IMapper<InterventionPlanDto, InterventionPlan>, InterventionPlanMapper>();
            services.AddScoped<IMapper<InterventionPlan, InterventionPlanDto>, InterventionPlanToDtoMapper>();

            services.AddScoped<IMapper<IndividualInterventionDto, IndividualIntervention>, IndividualInterventionMapper>();
            services.AddScoped<IMapper<IndividualIntervention, IndividualInterventionDto>, IndividualInterventionToDtoMapper>();

            services.AddScoped<IMapper<GroupInterventionDto, GroupIntervention>, GroupInterventionMapper>();
            services.AddScoped<IMapper<GroupIntervention, GroupInterventionDto>, GroupInterventionToDtoMapper>();

            services.AddScoped<IMapper<AssessmentDto, Assessment>, AssessmentMapper>();
            services.AddScoped<IMapper<Assessment, AssessmentDto>, AssessmentToDtoMapper>();

            services.AddScoped<IValidator<InterventionPlan>, InterventionPlanValidator>();
            services.AddScoped<IValidator<Assessment>, AssessementValidator>();

            return services;
        }
    }
}
