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
            _logger.LogDebug($"get all average risk = {Request.cohortId} {Request.pollId}");
            return await _studentRepository.GetStudentAverageRiskAsync(
                Request.cohortId,
                Request.pollId
            );
        }
    }
}
