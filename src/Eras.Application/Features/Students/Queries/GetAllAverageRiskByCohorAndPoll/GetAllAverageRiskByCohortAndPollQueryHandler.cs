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
            var result = await _studentRepository.GetStudentAverageRiskByCohortsAsync(
               Request.Pagination,
               Request.CohortIds,
               Request.PollUuid,
               Request.LastVersion
           );

            return result;
        }
    }
}
