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
            IStudentRepository studentRepository,
            ILogger<GetAllStudentsQueryHandler> logger
        )
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }

        public async Task<PagedResult<Student>> Handle(
            GetAllStudentsQuery request,
            CancellationToken cancellationToken
        )
        {
            var students = await _studentRepository.GetPagedAsync(
                request.Query.Page,
                request.Query.PageSize
            );
            var allstudents = await _studentRepository.GetAllAsync();

            var totalCount = allstudents.Count();

            PagedResult<Student> pagedResult = new PagedResult<Student>(
                totalCount,
                students.ToList()
            );

            return pagedResult;
        }
    }
}
