using MediatR;
using Eras.Domain.Entities;
using Eras.Application.Models.Response.Common;

namespace Eras.Application.Features.Evaluations.Queries;

public class GetEvaluationSummaryQuery : IRequest<GetQueryResponse<Evaluation?>>
{
    public required int EvaluationId { get; set; }
}
