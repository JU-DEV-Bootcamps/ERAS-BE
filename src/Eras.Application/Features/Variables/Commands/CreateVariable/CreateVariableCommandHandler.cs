using System;
using System.Collections.Generic;
using System.Linq;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Variables.Commands.CreateVariable
{
    public class CreateVariableCommandHandler : IRequestHandler<CreateVariableCommand, CreateCommandResponse<Variable>>
    {
        private readonly IVariableRepository _variableRepository;
        private readonly ILogger<CreateVariableCommandHandler> _logger;

        public CreateVariableCommandHandler(
            IVariableRepository variableRepository,
            ILogger<CreateVariableCommandHandler> logger)
        {

            _variableRepository = variableRepository;
            _logger = logger;
        }

        public async Task<CreateCommandResponse<Variable>> Handle(CreateVariableCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Variable? variableDB = await _variableRepository.GetByNameAsync(request.Variable.Name);
                if (variableDB != null) return new CreateCommandResponse<Variable>(variableDB, 0, "Success", true);


                Variable? variable = request.Variable?.ToDomain();
                if (variable!= null)
                {
                    variable.IdComponent = request.ComponentId;
                    variable.IdPoll = request.PollId;
                    Variable createdVariable = await _variableRepository.AddAsync(variable);
                    return new CreateCommandResponse<Variable>(createdVariable, 1, "Success", true);
                }
                return new CreateCommandResponse<Variable>(null,0, "Error", false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the variable: ");
                return new CreateCommandResponse<Variable>(null,0, "Error", false);
            }
        }
    }
}
