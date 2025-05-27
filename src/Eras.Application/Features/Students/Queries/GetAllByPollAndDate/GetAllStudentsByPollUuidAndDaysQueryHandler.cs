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
            IStudentRepository StudentRepository,
            ILogger<GetAllStudentsByPollUuidAndDaysQueryHandler> Logger
        )
        {
            _studentRepository = StudentRepository;
            _logger = Logger;
        }

        public async Task<PagedResult<Student>> Handle(GetAllStudentsByPollUuidAndDaysQuery Request, CancellationToken CancellationToken)
        {
            try
            {
                (IEnumerable<Student> students, int totalCount) = 
                    await _studentRepository.GetAllStudentsByPollUuidAndDaysQuery(
                        Request.Query.Page, 
                        Request.Query.PageSize, 
                        Request.PollUuid,
                        Request.Days);
 
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
