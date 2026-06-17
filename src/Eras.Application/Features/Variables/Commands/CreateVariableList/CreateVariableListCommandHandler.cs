using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.CommandsDTOS;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Variables.Commands.CreateVariableList;

public sealed class CreateVariableListCommandHandler : IRequestHandler<CreateVariableListCommand,
    CreateCommandResponse<List<Variable>>>
{
    private readonly IVariableRepository _variableRepository;
    private readonly ILogger<CreateVariableListCommandHandler> _logger;

    public CreateVariableListCommandHandler(IVariableRepository VariableRepository,
        ILogger<CreateVariableListCommandHandler> Logger)
    {
        _variableRepository = VariableRepository;
        _logger = Logger;
    }

    public async Task<CreateCommandResponse<List<Variable>>> Handle(CreateVariableListCommand Request,
        CancellationToken CancellationToken)
    {
        List<Variable> response = [];
        List<Variable> variablesToCreate = [];
        IEnumerable<Variable> existingVariables = await _variableRepository.GetAllAsync();
        try
        {
            foreach(VariableListCommandDTO variableDTO in Request.Variables)
            {
                Variable variable = variableDTO.variable.ToDomain();
                Variable? existingVariable = existingVariables.FirstOrDefault(
                    Var => variable.Name == Var.Name && variable.Position == Var.Position
                );
                if (existingVariable != null)
                {
                    response.Add(existingVariable);
                }
                else
                {
                    variable.IdComponent = variableDTO.ComponentId;
                    variable.IdPoll = variableDTO.PollId;
                    variablesToCreate.Add(variable);
                }
            }
            if (variablesToCreate.Count > 0)
            {
                response.AddRange(await _variableRepository.AddTrackedBatchAsync(variablesToCreate));
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occured creating variables.");
            return new CreateCommandResponse<List<Variable>>(null, 0, "Error creating variables", false);
        }

        return new CreateCommandResponse<List<Variable>>(response, response.Count, "Variables imported", true);
    }
}