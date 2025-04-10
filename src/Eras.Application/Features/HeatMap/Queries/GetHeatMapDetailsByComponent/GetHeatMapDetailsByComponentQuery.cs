using Eras.Application.DTOs.HeatMap;

using MediatR;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByComponent
{
    public sealed record GetHeatMapDetailsByComponentQuery(string ComponentName, int limit)
        : IRequest<List<StudentHeatMapDetailDto>>;
}
