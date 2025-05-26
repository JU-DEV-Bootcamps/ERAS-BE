using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Evaluations.Commands
{
    public class CreateEvaluationPollCommandHandler : IRequestHandler<CreateEvaluationPollCommand, CreateCommandResponse<Evaluation>>
    {
        private readonly IEvaluationPollRepository _evaluationPollRepository;
        private readonly ILogger<CreateEvaluationPollCommandHandler> _logger;
        public CreateEvaluationPollCommandHandler(IEvaluationPollRepository EvaluationPollRepository,
            ILogger<CreateEvaluationPollCommandHandler> Logger)
        {
            _evaluationPollRepository = EvaluationPollRepository;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<Evaluation>> Handle(CreateEvaluationPollCommand Request, CancellationToken CancellationToken)
        {
            try
            {
                Evaluation evaluation = Request.EvaluationDTO.ToDomain();
                evaluation.PollId = Request.EvaluationDTO.PollId;
                Evaluation response = await _evaluationPollRepository.AddAsync(evaluation);
                return new CreateCommandResponse<Evaluation>(response, 0, "EvaluationPoll created", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the evaluation: " + Request.EvaluationDTO.Name);
                return new CreateCommandResponse<Evaluation>(null, 0, "Error", false);
            }
        }
    }
}
