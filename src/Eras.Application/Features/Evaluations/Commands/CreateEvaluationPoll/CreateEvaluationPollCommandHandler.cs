using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Evaluations.Commands
{
    public class CreateEvaluationPollCommandHandler : IRequestHandler<CreateEvaluationPollCommand, CreateComandResponse<Evaluation>>
    {
        private readonly IEvaluationPollRepository _evaluationPollRepository;
        private readonly ILogger<CreateEvaluationPollCommandHandler> _logger;
        public CreateEvaluationPollCommandHandler(IEvaluationPollRepository evaluationPollRepository,
            ILogger<CreateEvaluationPollCommandHandler> logger)
        {
            _evaluationPollRepository = evaluationPollRepository;
            _logger = logger;
        }

        public async Task<CreateComandResponse<Evaluation>> Handle(CreateEvaluationPollCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Evaluation evaluation = request.EvaluationDTO.ToDomain();
                evaluation.PollId = request.EvaluationDTO.PollId;
                Evaluation response = await _evaluationPollRepository.AddAsync(evaluation);
                return new CreateComandResponse<Evaluation>(response, 0, "EvaluationPoll created", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the evaluation: " + request.EvaluationDTO.Name);
                return new CreateComandResponse<Evaluation>(null, 0, "Error", false);
            }
        }
    }
}
