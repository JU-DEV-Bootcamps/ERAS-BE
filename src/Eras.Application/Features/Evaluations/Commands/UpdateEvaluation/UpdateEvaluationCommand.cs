﻿using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Evaluations.Commands.UpdateEvaluation
{
    public class UpdateEvaluationCommand : IRequest<CreateCommandResponse<Evaluation>>
    {
        public EvaluationDTO EvaluationDTO = default!;
    }
}
