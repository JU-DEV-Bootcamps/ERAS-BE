using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Evaluations.Commands.CreateEvaluation;
using Eras.Application.Features.Evaluations.Commands.CreateEvaluationPoll;
using Eras.Application.Models;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Evaluations.Commands.UpdateEvaluation
{
    internal class UpdateEvaluationCommandHandler : IRequestHandler<UpdateEvaluationCommand, CreateComandResponse<Evaluation>>
    {
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly ILogger<UpdateEvaluationCommandHandler> _logger;
        private readonly IPollRepository _pollRepository;
        private readonly IMediator _mediator;
        public UpdateEvaluationCommandHandler(IEvaluationRepository evaluationRepository, IPollRepository pollRepository,
            ILogger<UpdateEvaluationCommandHandler> logger, IMediator mediator)
        {
            _evaluationRepository = evaluationRepository;
            _pollRepository = pollRepository;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<CreateComandResponse<Evaluation>> Handle(UpdateEvaluationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Evaluation? evaluation = await _evaluationRepository.GetByIdForUpdateAsync(request.EvaluationDTO.Id); // avoid duplicated istance with the same id

                if (evaluation == null)
                {
                    _logger.LogWarning("Evaluation with ID {Id} not found", request.EvaluationDTO.Id);
                    return new CreateComandResponse<Evaluation>(null, 0, "Evaluation not found", false);
                }

                // Fields always updatable
                evaluation.Name = request.EvaluationDTO.Name;
                evaluation.Status = request.EvaluationDTO.status;
                evaluation.StartDate = request.EvaluationDTO.StartDate;
                evaluation.EndDate = request.EvaluationDTO.EndDate;
                evaluation.Audit.ModifiedAt = DateTime.UtcNow;
                evaluation.Audit.ModifiedBy = "Controller";

                // Only updatable if they were never set
                if (string.IsNullOrEmpty(evaluation.PollName))
                {
                    if (string.IsNullOrEmpty(request.EvaluationDTO.PollName))
                    {
                        evaluation.PollName = string.Empty;
                    }
                    if (request.EvaluationDTO.pollId != 0)
                    {
                        evaluation.PollId = request.EvaluationDTO.pollId;
                    }
                    if (request.EvaluationDTO.EvaluationPollId != 0)
                    {
                        evaluation.EvaluationPollId = request.EvaluationDTO.EvaluationPollId;
                    }
                }
                await _evaluationRepository.UpdateAsync(evaluation);

                _logger.LogInformation("Successfully updated Evaluation ID {Id}", evaluation.Id);
                return new CreateComandResponse<Evaluation>(evaluation, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred updating the evaluation: " + request.EvaluationDTO.Name);
                return new CreateComandResponse<Evaluation>(null, 0, "Error", false);
            }
        }
    }
}
