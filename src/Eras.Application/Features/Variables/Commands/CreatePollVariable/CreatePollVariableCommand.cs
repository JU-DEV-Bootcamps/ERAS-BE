using Eras.Application.DTOs;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Variables.Commands.CreatePollVariable
{
    public class CreatePollVariableCommand : IRequest<CreateCommandResponse<Variable>>
    {
        public VariableDTO? Variable;
        public int PollId;
        public int VariableId;

    }
}
