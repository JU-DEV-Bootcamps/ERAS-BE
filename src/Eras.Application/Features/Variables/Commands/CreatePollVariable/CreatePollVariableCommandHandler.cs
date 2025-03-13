using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Variables.Commands.CreateVariable;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Variables.Commands.CreatePollVariable
{
    public class CreatePollVariableCommandHandler : IRequestHandler<CreatePollVariableCommand, CreateCommandResponse<Variable>>
    {
        private readonly IPollVariableRepository _pollVariableRepository;
        private readonly ILogger<CreatePollVariableCommandHandler> _logger;

        public CreatePollVariableCommandHandler(
            IPollVariableRepository pollVariableRepository,
            ILogger<CreatePollVariableCommandHandler> logger)
        {

            _pollVariableRepository = pollVariableRepository;
            _logger = logger;
        }

        public async Task<CreateCommandResponse<Variable>> Handle(CreatePollVariableCommand request, CancellationToken cancellationToken)
        {

            try
            {
                int pollId = request.PollId;
                int variableId = request.VariableId;
                Variable variableDB = await _pollVariableRepository.GetByPollIdAndVariableIdAsync(pollId, variableId);
                if (variableDB != null) return new CreateCommandResponse<Variable>(variableDB, 0, "Success", true);

                Variable createdVariable = request.Variable.ToDomain();
                createdVariable.IdPoll = pollId;
                createdVariable.Id = variableId;
                Variable response = await _pollVariableRepository.AddAsync(createdVariable);
                return new CreateCommandResponse<Variable>(response,1, "Success", true);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the variable: ");
                return new CreateCommandResponse<Variable>(null,0, "Error", false);
            }
        }
    }
}
