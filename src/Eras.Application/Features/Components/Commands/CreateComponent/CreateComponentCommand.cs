using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Utils;
using MediatR;

namespace Eras.Application.Features.Components.Commands.CreateCommand
{
    public class CreateComponentCommand : IRequest<BaseResponse>
    {
        public ComponentDTO? component;
    }
}
