using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.DTOs;
using Eras.Application.Models;
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
