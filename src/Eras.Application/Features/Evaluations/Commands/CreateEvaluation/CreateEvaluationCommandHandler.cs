﻿using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Eras.Application.Mappers;
using Eras.Domain.Common;
using Eras.Application.Models.Response.Common;

namespace Eras.Application.Features.Evaluations.Commands
{
    public class CreateEvaluationCommandHandler : IRequestHandler<CreateEvaluationCommand, CreateCommandResponse<Evaluation>>
    {
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly ILogger<CreateEvaluationCommandHandler> _logger;
        private readonly IPollRepository _pollRepository;
        private readonly IMediator _mediator;
        public CreateEvaluationCommandHandler(IEvaluationRepository evaluationRepository,IPollRepository pollRepository,
            ILogger<CreateEvaluationCommandHandler> logger, IMediator mediator)
        {
            _evaluationRepository = evaluationRepository;
            _pollRepository = pollRepository;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<CreateCommandResponse<Evaluation>> Handle(CreateEvaluationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string status = EvaluationConstants.EvaluationStatus.Pending.ToString();
                Evaluation? evaluation=null;
                Poll? poll = null;
                evaluation = await _evaluationRepository.GetByNameAsync(request.EvaluationDTO.Name);
                if (evaluation != null) return new CreateCommandResponse<Evaluation>(null, 0,
                    $"Evaluation with Name {request.EvaluationDTO.Name} already exists", false);


                if (!request.EvaluationDTO.PollName.Equals(string.Empty))
                    poll = await _pollRepository.GetByNameAsync(request.EvaluationDTO.PollName);
                evaluation = request.EvaluationDTO.ToDomain();
                evaluation.Audit = new AuditInfo()
                {
                    CreatedBy = "Controller",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };
                if (poll != null && !request.EvaluationDTO.PollName.Equals(string.Empty))
                {
                    evaluation.PollId = poll.Id;
                    status = EvaluationConstants.EvaluationStatus.Ready.ToString();
                }
                evaluation.Status = status;
                Evaluation response = await _evaluationRepository.AddAsync(evaluation);
                if (poll != null && status.Equals(EvaluationConstants.EvaluationStatus.Ready.ToString()))
                {
                    request.EvaluationDTO.PollId = poll.Id;
                    request.EvaluationDTO.Id = response.Id;
                    CreateEvaluationPollCommand evaluationPollCommand = new()
                    {
                        EvaluationDTO = request.EvaluationDTO
                    };
                    await _mediator.Send(evaluationPollCommand);
                }
                return new CreateCommandResponse<Evaluation>(response, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the evaluation: " + request.EvaluationDTO.Name);
                return new CreateCommandResponse<Evaluation>(null, 0, "Error", false);
            }
        }
    }
}
