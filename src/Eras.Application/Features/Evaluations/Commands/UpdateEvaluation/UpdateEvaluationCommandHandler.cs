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
                Evaluation? evaluationDB = await _evaluationRepository.GetByIdForUpdateAsync(request.EvaluationDTO.Id); // avoid duplicated istance with the same id

                if (evaluationDB == null)
                {
                    _logger.LogWarning("Evaluation with ID {Id} not found", request.EvaluationDTO.Id);
                    return new CreateComandResponse<Evaluation>(null, 0, "Evaluation not found", false);
                }

                // Fields always updatable
                evaluationDB.Name = request.EvaluationDTO.Name;
                evaluationDB.StartDate = request.EvaluationDTO.StartDate;
                evaluationDB.EndDate = request.EvaluationDTO.EndDate;
                evaluationDB.Audit.ModifiedAt = DateTime.UtcNow;
                evaluationDB.Audit.ModifiedBy = "Controller";

                // Only updatable if they were never set
                if (string.IsNullOrEmpty(evaluationDB.PollName) || evaluationDB.Status == "Incomplete")
                {
                    if (! string.IsNullOrEmpty(request.EvaluationDTO.PollName))
                    {
                        Poll?  poll = await _pollRepository.GetByNameAsync(request.EvaluationDTO.PollName);

                        evaluationDB.PollName = request.EvaluationDTO.PollName; 
                        if (poll != null)
                        {
                            try
                            {
                                evaluationDB.Status = "Complete";
                                request.EvaluationDTO.PollId = poll.Id;
                                request.EvaluationDTO.Id = evaluationDB.Id;
                                CreateEvaluationPollCommand evaluationPollCommand = new CreateEvaluationPollCommand()
                                {
                                    EvaluationDTO = request.EvaluationDTO
                                };
                                await _mediator.Send(evaluationPollCommand);

                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "An error occurred updating the evaluation: " + request.EvaluationDTO.Name);
                                return new CreateComandResponse<Evaluation>(null, 0, "Error", false);
                            }
                        }
                    }
                }
                await _evaluationRepository.UpdateAsync(evaluationDB);

                _logger.LogInformation("Successfully updated Evaluation ID {Id}", evaluationDB.Id);
                return new CreateComandResponse<Evaluation>(evaluationDB, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred updating the evaluation: " + request.EvaluationDTO.Name);
                return new CreateComandResponse<Evaluation>(null, 0, "Error", false);
            }
        }
    }
}
