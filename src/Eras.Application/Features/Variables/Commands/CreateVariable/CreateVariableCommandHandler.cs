using System;
using System.Collections.Generic;
using System.Linq;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Variables.Commands.CreateVariable
{
    public class CreateVariableCommandHandler : IRequestHandler<CreateVariableCommand, CreateComandResponse<Variable>>
    {
        private readonly IVariableRepository _variableRepository;
        private readonly IComponentRepository _componentRepository;
        private readonly ILogger<CreateVariableCommandHandler> _logger;

        public CreateVariableCommandHandler(
            IVariableRepository variableRepository,
            IComponentRepository componentRepository,
            ILogger<CreateVariableCommandHandler> logger)
        {

            _variableRepository = variableRepository;
            _componentRepository = componentRepository;
            _logger = logger;
        }

        public async Task<CreateComandResponse<Variable>> Handle(CreateVariableCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Variable? variable = request.Variable?.ToDomain(); 
                if (variable!= null)
                {
                    variable.IdComponent = request.ComponentId;
                    variable.IdPoll = request.PollId;
                    variable.PollVariableId = 0; // id del registro en tabla intermedia, se crea en db
                    variable.Audit = new AuditInfo()
                    {
                        CreatedBy = "Cosmic latte import",
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow,
                    };
                    Variable createdVariable = await _variableRepository.AddAsync(variable);
                    return new CreateComandResponse<Variable>(createdVariable, "Success", true);
                }
                return new CreateComandResponse<Variable>(null, "Error", false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the variable: ");
                return new CreateComandResponse<Variable>(null, "Error", false);
            }
        }
    }
}
