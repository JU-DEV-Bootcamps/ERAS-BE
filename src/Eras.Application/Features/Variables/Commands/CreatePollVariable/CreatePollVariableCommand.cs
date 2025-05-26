using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Variables.Commands.CreatePollVariable
{
    public class CreatePollVariableCommand : IRequest<CreateCommandResponse<Variable>>
    {
        public VariableDTO? Variable;
        public int PollId;
        public int VariableId;

    }
}
