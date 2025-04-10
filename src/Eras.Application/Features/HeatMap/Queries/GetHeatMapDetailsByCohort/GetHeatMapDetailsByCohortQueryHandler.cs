using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.HeatMap;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByCohort
{
    public class GetHeatMapDetailsByCohortQueryHandler
        : IRequestHandler<GetHeatMapDetailsByCohortQuery, List<StudentHeatMapDetailDto>>
    {
        private readonly ILogger<GetHeatMapDetailsByCohortQueryHandler> _logger;
        private readonly IStudentRepository _studentRepository;

        public GetHeatMapDetailsByCohortQueryHandler(
            IStudentRepository studentRepository,
            ILogger<GetHeatMapDetailsByCohortQueryHandler> logger
        )
        {
            _logger = logger;
            _studentRepository = studentRepository;
        }

        public async Task<List<StudentHeatMapDetailDto>> Handle(
            GetHeatMapDetailsByCohortQuery request,
            CancellationToken cancellationToken
        )
        {
            _logger.LogDebug($"Heat map details by cohort: {request.CohortId}");
            return await _studentRepository.GetStudentHeatMapDetailsByCohort(
                request.CohortId,
                request.limit
            );
        }
    }
}
