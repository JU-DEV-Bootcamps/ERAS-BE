using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Students.Queries.GetAll
{
    public class GetAllStudentsQueryHandler
        : IRequestHandler<GetAllStudentsQuery, PagedResult<Student>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<GetAllStudentsQueryHandler> _logger;

        public GetAllStudentsQueryHandler(
            IStudentRepository StudentRepository,
            ILogger<GetAllStudentsQueryHandler> Logger
        )
        {
            _studentRepository = StudentRepository;
            _logger = Logger;
        }

        public async Task<PagedResult<Student>> Handle(
            GetAllStudentsQuery Request,
            CancellationToken CancellationToken
        )
        {
            try
            {
                var students = await _studentRepository.GetPagedAsyncWithJoins(
                    Request.Query.Page,
                    Request.Query.PageSize
                );
                var totalCount = await _studentRepository.CountAsync();

                PagedResult<Student> pagedResult = new PagedResult<Student>(
                    totalCount,
                    students.ToList()
                );

                return pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all students: " + ex.Message);
                return new PagedResult<Student>(0, []);
            }
        }
    }
}
