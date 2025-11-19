using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using MediatR;

namespace Eras.Application.Features.Evaluations.Commands
{
    public class CreateEvaluationCommandHandler : IRequestHandler<CreateEvaluationCommand, CreateCommandResponse<Evaluation>>
    {
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly IPollRepository _pollRepository;
        private readonly IMediator _mediator;
        public CreateEvaluationCommandHandler(IEvaluationRepository EvaluationRepository,IPollRepository PollRepository, IMediator Mediator)
        {
            _evaluationRepository = EvaluationRepository;
            _pollRepository = PollRepository;
            _mediator = Mediator;
        }

        public async Task<CreateCommandResponse<Evaluation>> Handle(CreateEvaluationCommand Request, CancellationToken CancellationToken)
        {
            string status = EvaluationConstants.EvaluationStatus.Pending.ToString();
            Evaluation? evaluation=null;
            Poll? poll = null;
            evaluation = await _evaluationRepository.GetByNameAsync(Request.EvaluationDTO.Name);

            if (evaluation != null)
            {
                throw new ExistingEvaluationNameException(Request.EvaluationDTO.Name);
            }                                        

            if (!Request.EvaluationDTO.PollName.Equals(string.Empty))
            {
                poll = await _pollRepository.GetByParentIdAsync(Request.ParentId);
            }

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
    }
}
