using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Evaluations.Commands.UpdateEvaluation
{
    internal class UpdateEvaluationCommandHandler : IRequestHandler<UpdateEvaluationCommand, CreateCommandResponse<Evaluation>>
    {
        private readonly IEvaluationRepository _EvaluationRepository;
        private readonly ILogger<UpdateEvaluationCommandHandler> _logger;
        private readonly IPollRepository _PollRepository;
        private readonly IMediator _mediator;
        public UpdateEvaluationCommandHandler(IEvaluationRepository EvaluationRepository, IPollRepository PollRepository,
            ILogger<UpdateEvaluationCommandHandler> Logger, IMediator Mediator)
        {
            _EvaluationRepository = EvaluationRepository;
            _PollRepository = PollRepository;
            _logger = Logger;
            _mediator = Mediator;
        }

        public async Task<CreateCommandResponse<Evaluation>> Handle(UpdateEvaluationCommand Request, CancellationToken CancellationToken)
        {
            try
            {
                Evaluation? evaluationDB = await _EvaluationRepository.GetByIdForUpdateAsync(Request.EvaluationDTO.Id);

                if (evaluationDB == null)
                {
                    _logger.LogWarning("Evaluation with ID {Id} not found", Request.EvaluationDTO.Id);
                    return new CreateCommandResponse<Evaluation>(null, 0, "Evaluation not found", false);
                }

                // Fields always updatable
                evaluationDB.Name = Request.EvaluationDTO.Name;
                evaluationDB.StartDate = Request.EvaluationDTO.StartDate;
                evaluationDB.EndDate = Request.EvaluationDTO.EndDate;
                evaluationDB.Audit.ModifiedAt = DateTime.UtcNow;
                evaluationDB.Audit.ModifiedBy = "Controller";

                // Only updatable if they were never set
                if (string.IsNullOrEmpty(evaluationDB.PollName) || evaluationDB.Status == "Incomplete")
                {
                    if (! string.IsNullOrEmpty(Request.EvaluationDTO.PollName))
                    {
                        Poll?  poll = await _PollRepository.GetByNameAsync(Request.EvaluationDTO.PollName);

                        evaluationDB.PollName = Request.EvaluationDTO.PollName; 
                        if (poll != null)
                        {
                            try
                            {
                                evaluationDB.Status = "Complete";
                                Request.EvaluationDTO.PollId = poll.Id;
                                Request.EvaluationDTO.Id = evaluationDB.Id;
                                CreateEvaluationPollCommand evaluationPollCommand = new CreateEvaluationPollCommand()
                                {
                                    EvaluationDTO = Request.EvaluationDTO
                                };
                                await _mediator.Send(evaluationPollCommand);

                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "An error occurred updating the evaluation: " + Request.EvaluationDTO.Name);
                                return new CreateCommandResponse<Evaluation>(null, 0, "Error", false);
                            }
                        }
                    }
                }
                await _EvaluationRepository.UpdateAsync(evaluationDB);

                _logger.LogInformation("Successfully updated Evaluation ID {Id}", evaluationDB.Id);
                return new CreateCommandResponse<Evaluation>(evaluationDB, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred updating the evaluation: " + Request.EvaluationDTO.Name);
                return new CreateCommandResponse<Evaluation>(null, 0, "Error", false);
            }
        }
    }
}
