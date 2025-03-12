using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Evaluations.Commands.CreateEvaluation
{
    public class CreateEvaluationCommand : IRequest<CreateComandResponse<Evaluation>>
    {
        public EvaluationDTO EvaluationDTO = default!;
    }
}
