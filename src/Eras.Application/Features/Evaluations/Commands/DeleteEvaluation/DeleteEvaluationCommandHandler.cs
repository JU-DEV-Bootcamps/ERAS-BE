using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Models;
using Eras.Domain.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Evaluations.Commands.DeleteEvaluation
{

    public class DeleteEvaluationCommandHandler : IRequestHandler<DeleteEvaluationCommand, BaseResponse>
    {
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly ILogger<DeleteEvaluationCommandHandler> _logger;
        private readonly IPollRepository _pollRepository;
        private readonly IMediator _mediator;
        public DeleteEvaluationCommandHandler(IEvaluationRepository evaluationRepository, IPollRepository pollRepository,
            ILogger<DeleteEvaluationCommandHandler> logger, IMediator mediator)
        {
            _evaluationRepository = evaluationRepository;
            _pollRepository = pollRepository;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<BaseResponse> Handle(DeleteEvaluationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Evaluation? evaluation = await _evaluationRepository.GetByIdForUpdateAsync(request.id);

                if (evaluation == null)
                {
                    _logger.LogWarning("Evaluation with ID {Id} not found", request.id);
                    return new BaseResponse("Evaluation not found", false);
                }
                await _evaluationRepository.DeleteAsync(evaluation);
                return new BaseResponse("Evaluation deleted", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred deleteing the evaluation: " + request.id);
                return new CreateCommandResponse<Evaluation>(null, 0, "Error", false);
            }
        }
    }
}
