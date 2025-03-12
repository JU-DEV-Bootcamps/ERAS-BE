using MediatR;
using Eras.Domain.Entities;

namespace Eras.Application.Features.EvaluationProcess.Queries
{
    public class GetEvaluationProcessSummaryQuery: IRequest<List<Poll>>;
}
