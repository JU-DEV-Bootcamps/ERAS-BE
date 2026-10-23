using Eras.Application.Models.CommandsDTOS;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Variables.Commands.CreatePollVariableList;

public sealed class CreatePollVariableListCommand : IRequest<CreateCommandResponse<List<Variable>>>
{
    public required PollVariableListCommandDTO Variables;
}