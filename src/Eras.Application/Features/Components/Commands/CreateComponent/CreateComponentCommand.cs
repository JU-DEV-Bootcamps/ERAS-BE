﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Components.Commands.CreateCommand
{
    public class CreateComponentCommand : IRequest<CreateCommandResponse<Component>>
    {
        public ComponentDTO? Component;
    }
}
