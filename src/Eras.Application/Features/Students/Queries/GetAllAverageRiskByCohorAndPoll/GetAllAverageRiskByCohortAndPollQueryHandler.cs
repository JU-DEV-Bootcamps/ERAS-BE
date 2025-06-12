using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Student;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohorAndPoll
{
    public class GetAllAverageRiskByCohortAndPollQueryHandler
        : IRequestHandler<GetAllAverageRiskByCohortAndPollQuery, List<StudentAverageRiskDto>>
    {
        private readonly ILogger<GetAllAverageRiskByCohortAndPollQueryHandler> _logger;
        private readonly IStudentRepository _studentRepository;

        public GetAllAverageRiskByCohortAndPollQueryHandler(
            IStudentRepository StudentRepository,
            ILogger<GetAllAverageRiskByCohortAndPollQueryHandler> Logger
        )
        {
            _logger = Logger;
            _studentRepository = StudentRepository;
        }

        public async Task<List<StudentAverageRiskDto>> Handle(
            GetAllAverageRiskByCohortAndPollQuery Request,
            CancellationToken CancellationToken
        )
        {
            _logger.LogDebug($"get all average risk = {Request.CohortIds} {Request.PollUuid}");
            return await _studentRepository.GetStudentAverageRiskByCohortsAsync(
                Request.CohortIds,
                Request.PollUuid,
                Request.LastVersion
            );
        }
    }
}
