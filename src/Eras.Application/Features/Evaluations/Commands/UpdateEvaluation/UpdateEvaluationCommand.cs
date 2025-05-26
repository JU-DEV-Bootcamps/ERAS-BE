using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Evaluations.Commands.UpdateEvaluation
{
    public class UpdateEvaluationCommand : IRequest<CreateCommandResponse<Evaluation>>
    {
        public EvaluationDTO EvaluationDTO = default!;
    }
}
