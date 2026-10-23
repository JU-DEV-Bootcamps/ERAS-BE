using Eras.Application.Models.CommandsDTOS;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Variables.Commands.CreateVariableList;

public sealed class CreateVariableListCommand : IRequest<CreateCommandResponse<List<Variable>>>
{
    public required List<VariableListCommandDTO> Variables;
}