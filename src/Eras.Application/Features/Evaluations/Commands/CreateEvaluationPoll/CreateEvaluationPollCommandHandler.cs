using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Evaluations.Commands
{
    public class CreateEvaluationPollCommandHandler : IRequestHandler<CreateEvaluationPollCommand, CreateCommandResponse<Evaluation>>
    {
        private readonly IEvaluationPollRepository _evaluationPollRepository;
        private readonly ILogger<CreateEvaluationPollCommandHandler> _logger;
        public CreateEvaluationPollCommandHandler(IEvaluationPollRepository evaluationPollRepository,
            ILogger<CreateEvaluationPollCommandHandler> logger)
        {
            _evaluationPollRepository = evaluationPollRepository;
            _logger = logger;
        }

        public async Task<CreateCommandResponse<Evaluation>> Handle(CreateEvaluationPollCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Evaluation evaluation = request.EvaluationDTO.ToDomain();
                evaluation.PollId = request.EvaluationDTO.PollId;
                Evaluation response = await _evaluationPollRepository.AddAsync(evaluation);
                return new CreateCommandResponse<Evaluation>(response, 0, "EvaluationPoll created", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the evaluation: " + request.EvaluationDTO.Name);
                return new CreateCommandResponse<Evaluation>(null, 0, "Error", false);
            }
        }
    }
}
