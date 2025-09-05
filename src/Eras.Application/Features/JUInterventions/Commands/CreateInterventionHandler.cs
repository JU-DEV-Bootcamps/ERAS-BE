using Eras.Application.Mappers;
using MediatR;
using Microsoft.Extensions.Logging;
using Eras.Domain.Entities;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Enums;
using Eras.Application.Contracts.Persistence;

namespace Eras.Application.Features.Interventions.Commands.CreateIntervention
{
    public class CreateInterventionCommandHandler : IRequestHandler<CreateInterventionCommand, CreateCommandResponse<JUIntervention>>
    {
        private readonly IInterventionRepository _interventionRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IRemissionRepository _remissionRepository;
        private readonly ILogger<CreateInterventionCommandHandler> _logger;

        public CreateInterventionCommandHandler(
            IInterventionRepository InterventionRepository,
            IStudentRepository StudentRepository,
            IRemissionRepository RemissionRepository,
            ILogger<CreateInterventionCommandHandler> Logger)
        {
            _interventionRepository = InterventionRepository;
            _studentRepository = StudentRepository;
            _remissionRepository = RemissionRepository;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<JUIntervention>> Handle(CreateInterventionCommand Request, CancellationToken CancellationToken)
        {
            try
            {
                if (Request.Intervention.Id != 0)
                {
                    JUIntervention? entityInDB = await _interventionRepository.GetByIdAsync(Request.Intervention.Id);
                    if (entityInDB != null)
                    {
                        return new CreateCommandResponse<JUIntervention>(new JUIntervention(), "Entity already exists", false,
                            CommandEnums.CommandResultStatus.AlreadyExists);
                    }
                }

                JUIntervention intervention = Request.Intervention.ToDomain();
                JUIntervention response = await _interventionRepository.AddAsync(intervention);
                return new CreateCommandResponse<JUIntervention>(response, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the intervention: " + Request.Intervention.Id);
                return new CreateCommandResponse<JUIntervention>(new JUIntervention(), 0, "Error creating intervention", false);
            }
        }
    }
}
