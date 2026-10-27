using Eras.Application.DTOs.HeatMap;

using MediatR;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByCohort
{
    public sealed record GetHeatMapDetailsByCohortQuery(string CohortId, int limit)
        : IRequest<List<StudentHeatMapDetailDto>>;
}
