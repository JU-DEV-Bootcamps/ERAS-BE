using Eras.Application.DTOs.HeatMapDTOs;
using MediatR;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents
{
    internal class GetHeatMapDataByAllComponentsHandler : IRequestHandler<GetHeatMapDataByAllComponentsQuery, HeatMapComponentsResponseDTO>
    {
        public Task<HeatMapComponentsResponseDTO> Handle(GetHeatMapDataByAllComponentsQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
