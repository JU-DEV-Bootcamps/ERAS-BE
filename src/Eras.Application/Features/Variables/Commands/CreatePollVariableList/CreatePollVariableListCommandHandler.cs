using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Variables.Commands.CreatePollVariableList;

public sealed class CreatePollVariableListCommandHandler : IRequestHandler<CreatePollVariableListCommand,
    CreateCommandResponse<List<Variable>>>
{
    private readonly IPollVariableRepository _pollVariableRepository;
    private readonly ILogger<CreatePollVariableListCommandHandler> _logger;

    public CreatePollVariableListCommandHandler(IPollVariableRepository PollVariableRepository,
        ILogger<CreatePollVariableListCommandHandler> Logger)
    {
        _pollVariableRepository = PollVariableRepository;
        _logger = Logger;
    }

    public async Task<CreateCommandResponse<List<Variable>>> Handle(CreatePollVariableListCommand Request,
        CancellationToken CancellationToken)
    {
        var pollId = Request.Variables.pollId;
        List<Variable> response = [];
        List<Variable> pollVariablesToCreate = [];
        List<Variable> mappedPollVariables = await _pollVariableRepository.GetAllWithVariablesAsync();
        try
        {
            foreach(Variable variable in Request.Variables.variables)
            {
                Variable? existingVariable = mappedPollVariables.FirstOrDefault(
                    PollVar => variable.Id == PollVar.Id && pollId == PollVar.IdPoll
                );
                if (existingVariable != null)
                {
                    response.Add(existingVariable);
                }
                else
                {
                    variable.IdPoll = pollId;
                    pollVariablesToCreate.Add(variable);
                }
            }

            if (pollVariablesToCreate.Count > 0)
            {
                response.AddRange(await _pollVariableRepository.AddBatchPollVariablesAsync(pollVariablesToCreate));
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error creating Poll Variables");
            return new CreateCommandResponse<List<Variable>>(null, 0, "Error creating Poll Variables", false);
        }
        return new CreateCommandResponse<List<Variable>>(response, response.Count, "Poll Variables created successfully", true);
    }
}