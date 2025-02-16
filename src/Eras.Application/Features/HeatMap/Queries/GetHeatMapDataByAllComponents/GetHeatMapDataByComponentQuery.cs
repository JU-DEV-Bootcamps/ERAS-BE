using Eras.Application.DTOs.HeatMapDTOs;
using MediatR;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents
{
    public class GetHeatMapDataByAllComponentsQuery : IRequest<HeatMapComponentsResponseDTO>
    {
    }
}
