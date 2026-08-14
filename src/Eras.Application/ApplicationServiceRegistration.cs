using System.Reflection;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement.StatusManagement;
using Eras.Domain.Entities.AssessmentManagement.Validators;
using Eras.Domain.Entities.FeatureFlagManagement;

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
            Services.AddScoped<PollOrchestratorService>();
            Services.AddScoped<IPollOrchestratorServiceV2, PollOrchestratorServiceV2>();
            Services.AddScoped<PollStructureImporter>();
            Services.AddScoped<StudentImporter>();
            Services.AddScoped<PollInstanceImporter>();
            Services.AddScoped<IImportJobService, ImportJobService>();
            Services.AddScoped<EvaluationStatusUpdater>();
            Services.AddScoped<IFeatureFlagService, FeatureFlagService>();
            Services.ConfigureMappers();
            Services.ConfigureValidators();
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

            return services;
        }

        public static IServiceCollection ConfigureValidators(this IServiceCollection Services)
        {
            Services.AddScoped<IValidator<InterventionPlan>, InterventionPlanValidator>();
            Services.AddScoped<IValidator<Assessment>, AssessementValidator>();
            Services.AddScoped<IValidator<StatusTransitionRequest<InterventionStatus>>, InterventionStatusTransitionValidator>();
            Services.AddScoped<IValidator<StatusTransitionRequest<AssessmentStatus>>, AssessmentStatusTransitionValidator>();
            Services.AddScoped<IValidator<FeatureFlag>, FeatureFlagValidator>();

            return Services;  
        }
    }
}
