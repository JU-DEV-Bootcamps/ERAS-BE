using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Variables.Commands.CreateVariable
{
    public class CreateVariableCommand : IRequest<CreateCommandResponse<Variable>>
    {
        public VariableDTO? Variable;
        public int ComponentId;
        public int PollId;
    }
}
