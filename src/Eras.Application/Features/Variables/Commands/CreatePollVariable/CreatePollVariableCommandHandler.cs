using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Variables.Commands.CreatePollVariable
{
    public class CreatePollVariableCommandHandler : IRequestHandler<CreatePollVariableCommand, CreateCommandResponse<Variable>>
    {
        private readonly IPollVariableRepository _pollVariableRepository;
        private readonly ILogger<CreatePollVariableCommandHandler> _logger;

        public CreatePollVariableCommandHandler(
            IPollVariableRepository PollVariableRepository,
            ILogger<CreatePollVariableCommandHandler> Logger)
        {

            _pollVariableRepository = PollVariableRepository;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<Variable>> Handle(CreatePollVariableCommand Request, CancellationToken CancellationToken)
        {
            try
            {
                int pollId = Request.PollId;
                int variableId = Request.VariableId;

                Variable? variableDB = await _pollVariableRepository.GetByPollIdAndVariableIdAsync(pollId, variableId);
                if (variableDB != null) return new CreateCommandResponse<Variable>(variableDB, 0, "Success", true);

                if (Request.Variable != null)
                {
                    Variable? createdVariable = Request.Variable.ToDomain();
                    createdVariable.IdPoll = pollId;
                    createdVariable.Id = variableId;
                    Variable response = await _pollVariableRepository.AddAsync(createdVariable);
                    return new CreateCommandResponse<Variable>(response, 1, "Success", true);
                } else
                {
                    _logger.LogError("An error occurred creating the variable: Request.Variable is null.");
                    return new CreateCommandResponse<Variable>(null, 0, "Error", false);
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the variable: ");
                return new CreateCommandResponse<Variable>(null, 0, "Error", false);
            }
        }
    }
}
