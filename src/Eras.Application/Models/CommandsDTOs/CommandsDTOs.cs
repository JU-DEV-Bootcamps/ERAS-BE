using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Models.CommandsDTOS;

public sealed class PollVariableListCommandDTO
{
    public required List<Variable> variables;
    public int pollId;
}

public sealed class VariableListCommandDTO
{
    public required VariableDTO variable;
    public int ComponentId;
    public int PollId;
}

