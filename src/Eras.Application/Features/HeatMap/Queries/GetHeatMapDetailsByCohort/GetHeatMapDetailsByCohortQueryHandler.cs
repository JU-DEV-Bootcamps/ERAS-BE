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
            IStudentRepository StudentRepository,
            ILogger<GetHeatMapDetailsByCohortQueryHandler> Logger
        )
        {
            _logger = Logger;
            _studentRepository = StudentRepository;
        }

        public async Task<List<StudentHeatMapDetailDto>> Handle(
            GetHeatMapDetailsByCohortQuery Request,
            CancellationToken CancellationToken
        )
        {
            _logger.LogDebug($"Heat map details by cohort: {Request.CohortId}");
            return await _studentRepository.GetStudentHeatMapDetailsByCohort(
                Request.CohortId,
                Request.limit
            );
        }
    }
}
