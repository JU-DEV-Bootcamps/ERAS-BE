using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Evaluations.Commands.DeleteEvaluation
{
    
    public class DeleteEvaluationCommandHandler : IRequestHandler<DeleteEvaluationCommand,BaseResponse>
    {
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly ILogger<DeleteEvaluationCommandHandler> _logger;
        private readonly IPollRepository _pollRepository;
        private readonly IMediator _mediator;
        public DeleteEvaluationCommandHandler(IEvaluationRepository EvaluationRepository, IPollRepository PollRepository,
            ILogger<DeleteEvaluationCommandHandler> Logger, IMediator Mediator)
        {
            _evaluationRepository = EvaluationRepository;
            _pollRepository = PollRepository;
            _logger = Logger;
            _mediator = Mediator;
        }

        public async Task<BaseResponse> Handle(DeleteEvaluationCommand Request, CancellationToken CancellationToken)
        {
            try
            {
                Evaluation? evaluation = await _evaluationRepository.GetByIdForUpdateAsync(Request.id); 

                if (evaluation == null)
                {
                    _logger.LogWarning("Evaluation with ID {Id} not found", Request.id);
                    return new BaseResponse("Evaluation not found", false);
                }
                await _evaluationRepository.DeleteAsync(evaluation);
                return new BaseResponse("Evaluation deleted", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred deleteing the evaluation: " + Request.id);
                return new CreateCommandResponse<Evaluation>(null, 0, "Error", false);
            }
        }
    }
}
