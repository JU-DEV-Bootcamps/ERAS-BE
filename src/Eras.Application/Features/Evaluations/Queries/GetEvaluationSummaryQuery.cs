using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Evaluations.Queries;

public class GetEvaluationSummaryQuery : IRequest<GetQueryResponse<Evaluation?>>
{
    public required int EvaluationId { get; set; }
}
