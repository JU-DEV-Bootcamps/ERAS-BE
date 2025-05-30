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
            IStudentRepository StudentRepository,
            ILogger<GetHeatMapDetailsByComponentQueryHandler> Logger
        )
        {
            _logger = Logger;
            _studentRepository = StudentRepository;
        }

        public async Task<List<StudentHeatMapDetailDto>> Handle(
            GetHeatMapDetailsByComponentQuery Request,
            CancellationToken CancellationToken
        )
        {
            _logger.LogDebug($"heatmap details by component = {Request.ComponentName}");

            return await _studentRepository.GetStudentHeatMapDetailsByComponent(
                Request.ComponentName,
                Request.limit
            );
        }
    }
}
