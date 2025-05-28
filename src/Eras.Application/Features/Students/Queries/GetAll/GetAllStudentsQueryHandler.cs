using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Controllers.StudentsController;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Students.Queries.GetAll
{
    public class GetAllStudentsQueryHandler
        : IRequestHandler<GetAllStudentsQuery, PagedResult<GetAllStudentsQueryResponse>>
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

        public async Task<PagedResult<GetAllStudentsQueryResponse>> Handle(
            GetAllStudentsQuery request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var students = await _studentRepository.GetPagedAsyncWithJoins(
                    request.Query.Page,
                    request.Query.PageSize
                );
                var totalCount = await _studentRepository.CountAsync();

                var studentsResponses = students.Select(Student => new GetAllStudentsQueryResponse 
                {
                    Id = Student.Id,
                    Name = Student.Name,
                    Email = Student.Email,
                    IsImported = Student.IsImported,
                }).ToList();

                PagedResult<GetAllStudentsQueryResponse> pagedResult = new PagedResult<GetAllStudentsQueryResponse>(
                    totalCount,
                    studentsResponses
                );

                return pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all students: " + ex.Message);
                return new PagedResult<GetAllStudentsQueryResponse>(0, []);
            }
        }
    }
}
