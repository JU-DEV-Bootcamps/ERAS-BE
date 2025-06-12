using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Student;
using Eras.Application.Utils;
using Microsoft.Extensions.Logging;

using MediatR;

namespace Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohorAndPoll
{
    public class GetAllAverageRiskByCohortAndPollQueryHandler : IRequestHandler<GetAllAverageRiskByCohortAndPollQuery, PagedResult<StudentAverageRiskDto>>
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

        public async Task<PagedResult<StudentAverageRiskDto>> Handle(
            GetAllAverageRiskByCohortAndPollQuery Request,
            CancellationToken CancellationToken
        )
        {
<<<<<<< HEAD
            _logger.LogDebug($"get all average risk = {Request.CohortIds} {Request.PollUuid}");
            return await _studentRepository.GetStudentAverageRiskByCohortsAsync(
                Request.CohortIds,
                Request.PollUuid,
                Request.LastVersion
            );
=======
            var result = await _studentRepository.GetStudentAverageRiskByCohortsAsync(
               Request.Pagination,
               Request.CohortIds,
               Request.PollUuid
           );

            return result;
>>>>>>> e9b0c55b5e44d163e90bd31a48fecfe3e2bc471d
        }
    }
}
