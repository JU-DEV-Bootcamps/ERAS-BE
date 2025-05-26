using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Students.Queries.GetAllByPollAndDate
{
    public class GetAllStudentsByPollUuidAndDaysQueryHandler : IRequestHandler<GetAllStudentsByPollUuidAndDaysQuery, PagedResult<Student>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<GetAllStudentsByPollUuidAndDaysQueryHandler> _logger;


        public GetAllStudentsByPollUuidAndDaysQueryHandler(
            IStudentRepository studentRepository,
            ILogger<GetAllStudentsByPollUuidAndDaysQueryHandler> logger
        )
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }

        public async Task<PagedResult<Student>> Handle(GetAllStudentsByPollUuidAndDaysQuery request, CancellationToken CancellationToken)
        {
            try
            {
                (IEnumerable<Student> students, int totalCount) = 
                    await _studentRepository.GetAllStudentsByPollUuidAndDaysQuery(
                        request.Query.Page, 
                        request.Query.PageSize, 
                        request.PollUuid,
                        request.Days);
 
                return new PagedResult<Student>(totalCount, students.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting students: " + ex.Message);
                return new PagedResult<Student>(0, []);
            }
        }
    }
}
