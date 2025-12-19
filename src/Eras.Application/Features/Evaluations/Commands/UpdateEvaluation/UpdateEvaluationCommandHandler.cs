using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Evaluations.Commands.UpdateEvaluation
{
    internal class UpdateEvaluationCommandHandler : IRequestHandler<UpdateEvaluationCommand, CreateCommandResponse<Evaluation>>
    {
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly ILogger<UpdateEvaluationCommandHandler> _logger;
        private readonly IPollRepository _PollRepository;
        private readonly IMediator _mediator;
        public UpdateEvaluationCommandHandler(IEvaluationRepository EvaluationRepository, IPollRepository PollRepository,
            ILogger<UpdateEvaluationCommandHandler> Logger, IMediator Mediator)
        {
            _evaluationRepository = EvaluationRepository;
            _PollRepository = PollRepository;
            _logger = Logger;
            _mediator = Mediator;
        }

        public async Task<CreateCommandResponse<Evaluation>> Handle(UpdateEvaluationCommand Request, CancellationToken CancellationToken)
        {
            Evaluation? evaluationDB =
                await _evaluationRepository.GetByIdForUpdateAsync(Request.EvaluationDTO.Id)
                ?? throw new NotFoundException($"Evaluation with ID {Request.EvaluationDTO.Id} not found");

            Evaluation? evaluation = await _evaluationRepository.GetByNameForUpdateAsync(Request.EvaluationDTO.Id, Request.EvaluationDTO.Name);
            if (evaluation != null) 
            {
                throw new ExistingEvaluationNameException(Request.EvaluationDTO.Name);
            }                

            // Fields always updatable
            evaluationDB.Name = Request.EvaluationDTO.Name;
            evaluationDB.StartDate = Request.EvaluationDTO.StartDate;
            evaluationDB.EndDate = Request.EvaluationDTO.EndDate;
            evaluationDB.Audit.ModifiedAt = DateTime.UtcNow;
            evaluationDB.Audit.ModifiedBy = "Controller";
            evaluationDB.Country = Request.EvaluationDTO.Country;

            // Only updatable if they were never set
            if (string.IsNullOrEmpty(evaluationDB.PollName) || evaluationDB.Status == "Incomplete")
            {
                if (!string.IsNullOrEmpty(Request.EvaluationDTO.PollName))
                {
                    Poll? poll = await _PollRepository.GetByNameAsync(Request.EvaluationDTO.PollName);

                    evaluationDB.PollName = Request.EvaluationDTO.PollName;
                    if (poll != null)
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
                }
            }
            await _evaluationRepository.UpdateAsync(evaluationDB);

            _logger.LogInformation("Successfully updated Evaluation ID {Id}", evaluationDB.Id);
            return new CreateCommandResponse<Evaluation>(evaluationDB, 1, "Success", true);
        }
    }
}
