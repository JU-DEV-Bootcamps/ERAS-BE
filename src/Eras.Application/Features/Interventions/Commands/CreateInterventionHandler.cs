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
        private readonly ILogger<CreateInterventionCommandHandler> _logger;

        public CreateInterventionCommandHandler(IInterventionRepository InterventionRepository, ILogger<CreateInterventionCommandHandler> Logger)
        {
            _interventionRepository = InterventionRepository;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<JUIntervention>> Handle(CreateInterventionCommand Request, CancellationToken CancellationToken)
        {
            try
            {
                int? Id = Request.Intervention.Id;
                
                if (Id != null)
                {
                    JUIntervention? entityInDB = await _interventionRepository.GetByIdAsync((int) Id);
                    if (entityInDB != null) return new CreateCommandResponse<JUIntervention>(new JUIntervention(), "Entity already exists", false,
                        CommandEnums.CommandResultStatus.AlreadyExists);
                }
                
                JUIntervention intervention = Request.Intervention.ToDomain();
                JUIntervention response = await _interventionRepository.AddAsync(intervention);
                return new CreateCommandResponse<JUIntervention>(response, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the intervention: " + Request.Intervention.Id);
                return new CreateCommandResponse<JUIntervention>(new JUIntervention(), "Error", false, CommandEnums.CommandResultStatus.Error);
            }
        }
    }
}
