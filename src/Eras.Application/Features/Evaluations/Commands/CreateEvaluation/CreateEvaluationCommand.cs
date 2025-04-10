using Eras.Application.DTOs;
using Eras.Application.Models;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Evaluations.Commands
{
    public class CreateEvaluationCommand : IRequest<CreateCommandResponse<Evaluation>>
    {
        public EvaluationDTO EvaluationDTO = default!;
    }
}
