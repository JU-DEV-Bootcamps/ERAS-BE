using Eras.Application.Models.HeatMap;
using MediatR;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapByPollIdAndVariableIds
{
    public sealed record GetHeatMapByPollIdAndVariableIdsQuery(
        string pollUuid,
        List<int> variableIds
    ) : IRequest<List<HeatMapBaseData>>;
}
