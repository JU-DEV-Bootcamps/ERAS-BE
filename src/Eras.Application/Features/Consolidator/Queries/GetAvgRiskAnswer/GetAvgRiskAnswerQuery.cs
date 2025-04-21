using Eras.Application.Models.Response;
using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.GetAvgRiskAnswer;

public class GetAvgRiskAnswerQuery: IRequest<BaseResponse>
{
    public required List<int> StudentIds { get; set; }
    public required List<int> AnswerIds { get; set; }
}
