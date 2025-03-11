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
            IStudentRepository studentRepository,
            ILogger<GetAllAverageRiskByCohortAndPollQueryHandler> logger
        )
        {
            _logger = logger;
            _studentRepository = studentRepository;
        }

        public async Task<List<StudentAverageRiskDto>> Handle(
            GetAllAverageRiskByCohortAndPollQuery request,
            CancellationToken cancellationToken
        )
        {
            _logger.LogDebug($"get all average risk = {request.cohortId} {request.pollId}");
            return await _studentRepository.GetStudentAverageRiskAsync(
                request.cohortId,
                request.pollId
            );
        }
    }
}
