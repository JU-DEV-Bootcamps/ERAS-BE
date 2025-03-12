using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.DTOs;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Evaluations.Commands.CreateEvaluationPoll
{
    public class CreateEvaluationPollCommand : IRequest<CreateComandResponse<Evaluation>>
    {
        public EvaluationDTO EvaluationDTO = default!;
    }
}
