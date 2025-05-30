using Eras.Application.Models.Response;
using MediatR;

namespace Eras.Application.Features.Evaluations.Commands.DeleteEvaluation
{
    public class DeleteEvaluationCommand : IRequest<BaseResponse>
    {
        public int id;
    }
}