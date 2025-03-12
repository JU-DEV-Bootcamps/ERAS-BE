using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Evaluations.Commands.CreateEvaluation;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Evaluations.Commands.CreateEvaluationPoll
{
    internal class CreateEvaluationPollCommandHandler : IRequestHandler<CreateEvaluationPollCommand, CreateComandResponse<Evaluation>>
    {
        private readonly IEvaluationPollRepository _evaluationPollRepository;
        private readonly ILogger<CreateEvaluationCommandHandler> _logger;
        public CreateEvaluationPollCommandHandler(IEvaluationPollRepository evaluationPollRepository,
            ILogger<CreateEvaluationCommandHandler> logger)
        {
            _evaluationPollRepository = evaluationPollRepository;
            _logger = logger;
        }

        public async Task<CreateComandResponse<Evaluation>> Handle(CreateEvaluationPollCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Evaluation evaluation = request.EvaluationDTO.ToDomain();
                evaluation.PollId = request.EvaluationDTO.pollId;
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
