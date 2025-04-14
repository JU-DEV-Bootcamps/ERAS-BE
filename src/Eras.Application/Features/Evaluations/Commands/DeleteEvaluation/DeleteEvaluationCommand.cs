using Eras.Application.Models.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Evaluations.Commands.DeleteEvaluation
{
    public class DeleteEvaluationCommand : IRequest<BaseResponse>
    {
        public int id;
    }
}