using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Student;
using Eras.Application.Utils;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohorAndPoll
{
    public class GetAllAverageRiskByCohortsAndPollQueryHandler
        : IRequestHandler<GetAllAverageRiskByCohortsAndPollQuery, PagedResult<StudentAverageRiskDto>>
    {
        private readonly ILogger<GetAllAverageRiskByCohortsAndPollQueryHandler> _logger;
        private readonly IStudentRepository _studentRepository;

        public GetAllAverageRiskByCohortsAndPollQueryHandler(
            IStudentRepository StudentRepository,
            ILogger<GetAllAverageRiskByCohortsAndPollQueryHandler> Logger
        )
        {
            _logger = Logger;
            _studentRepository = StudentRepository;
        }

        public async Task<PagedResult<StudentAverageRiskDto>> Handle(
            GetAllAverageRiskByCohortsAndPollQuery Request,
            CancellationToken CancellationToken
        )
        {
            _logger.LogDebug($"get all average risk = {Request.cohortIds} {Request.PollUuid}");
            return await _studentRepository.GetStudentAverageRiskByCohortsAsync(
                Request.Pagination,
                Request.cohortIds,
                Request.PollUuid,
                Request.LastVersion
            );
        }
    }
}
