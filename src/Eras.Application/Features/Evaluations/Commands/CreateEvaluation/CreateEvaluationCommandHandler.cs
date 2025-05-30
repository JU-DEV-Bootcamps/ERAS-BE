using Eras.Application.Contracts.Persistence;
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
        public CreateEvaluationCommandHandler(IEvaluationRepository EvaluationRepository,IPollRepository PollRepository,
            ILogger<CreateEvaluationCommandHandler> Logger, IMediator Mediator)
        {
            _evaluationRepository = EvaluationRepository;
            _pollRepository = PollRepository;
            _logger = Logger;
            _mediator = Mediator;
        }

        public async Task<CreateCommandResponse<Evaluation>> Handle(CreateEvaluationCommand Request, CancellationToken CancellationToken)
        {
            try
            {
                string status = EvaluationConstants.EvaluationStatus.Pending.ToString();
                Evaluation? evaluation=null;
                Poll? poll = null;
                evaluation = await _evaluationRepository.GetByNameAsync(Request.EvaluationDTO.Name);
                if (evaluation != null) return new CreateCommandResponse<Evaluation>(null, 0,
                    $"Evaluation with Name {Request.EvaluationDTO.Name} already exists", false);


                if (!Request.EvaluationDTO.PollName.Equals(string.Empty))
                    poll = await _pollRepository.GetByNameAsync(Request.EvaluationDTO.PollName);
                evaluation = Request.EvaluationDTO.ToDomain();
                evaluation.Audit = new AuditInfo()
                {
                    CreatedBy = "Controller",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };
                if (poll != null && !Request.EvaluationDTO.PollName.Equals(string.Empty))
                {
                    evaluation.PollId = poll.Id;
                    status = EvaluationConstants.EvaluationStatus.Ready.ToString();
                }
                evaluation.Status = status;
                Evaluation response = await _evaluationRepository.AddAsync(evaluation);
                if (poll != null && status.Equals(EvaluationConstants.EvaluationStatus.Ready.ToString()))
                {
                    Request.EvaluationDTO.PollId = poll.Id;
                    Request.EvaluationDTO.Id = response.Id;
                    CreateEvaluationPollCommand evaluationPollCommand = new()
                    {
                        EvaluationDTO = Request.EvaluationDTO
                    };
                    await _mediator.Send(evaluationPollCommand);
                }
                return new CreateCommandResponse<Evaluation>(response, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the evaluation: " + Request.EvaluationDTO.Name);
                return new CreateCommandResponse<Evaluation>(null, 0, "Error", false);
            }
        }
    }
}
