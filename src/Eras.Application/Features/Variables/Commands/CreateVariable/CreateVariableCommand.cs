using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.DTOs;
using Eras.Application.Utils;
using MediatR;

namespace Eras.Application.Features.Variables.Commands.CreateVariable
{
    public class CreateVariableCommand : IRequest<BaseResponse>
    {
        public VariableDTO? component;
    }
}
