using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.HeatMap;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByComponent
{
    public class GetHeatMapDetailsByComponentQueryHandler
        : IRequestHandler<GetHeatMapDetailsByComponentQuery, List<StudentHeatMapDetailDto>>
    {
        private readonly ILogger<GetHeatMapDetailsByComponentQueryHandler> _logger;
        private readonly IStudentRepository _studentRepository;

        public GetHeatMapDetailsByComponentQueryHandler(
            IStudentRepository studentRepository,
            ILogger<GetHeatMapDetailsByComponentQueryHandler> logger
        )
        {
            _logger = logger;
            _studentRepository = studentRepository;
        }

        public async Task<List<StudentHeatMapDetailDto>> Handle(
            GetHeatMapDetailsByComponentQuery request,
            CancellationToken CancellationToken
        )
        {
            _logger.LogDebug($"heatmap details by component = {request.ComponentName}");

            return await _studentRepository.GetStudentHeatMapDetailsByComponent(
                request.ComponentName,
                request.limit
            );
        }
    }
}
