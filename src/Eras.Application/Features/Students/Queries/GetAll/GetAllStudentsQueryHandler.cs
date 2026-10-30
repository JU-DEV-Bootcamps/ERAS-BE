using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
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
        private readonly IStudentProfileRepository _profileRepository;
        private readonly IStudentAccessService _accessService;
        private readonly ILogger<GetAllStudentsQueryHandler> _logger;

        public GetAllStudentsQueryHandler(
            IStudentRepository StudentRepository,
            IStudentProfileRepository ProfileRepository,
            IStudentAccessService AccessService,
            ILogger<GetAllStudentsQueryHandler> Logger
        )
        {
            _studentRepository = StudentRepository;
            _profileRepository = ProfileRepository;
            _accessService = AccessService;
            _logger = Logger;
        }

        public async Task<PagedResult<GetAllStudentsQueryResponse>> Handle(
            GetAllStudentsQuery Request,
            CancellationToken CancellationToken
        )
        {
            // Resolved outside the try below so a forbidden user gets a 403 instead of an empty page (ADR-0004).
            IReadOnlyCollection<int>? allowedStudentIds = await _accessService.GetVisibleStudentIdsAsync(CancellationToken);

            try
            {
                var (students, totalCount) = await _studentRepository.GetPagedAsyncWithJoins(
                    Request.Query.Page,
                    Request.Query.PageSize,
                    Request.Search,
                    allowedStudentIds
                );
                HashSet<int> studentsWithProfile = await _profileRepository
                    .GetStudentIdsWithProfileAsync(students.Select(Student => Student.Id).ToList());

                var studentsResponses = students.Select(Student => new GetAllStudentsQueryResponse
                {
                    Id = Student.Id,
                    Name = Student.Name,
                    Email = Student.Email,
                    Uuid = Student.Uuid,
                    IsImported = Student.IsImported,
                    HasProfile = studentsWithProfile.Contains(Student.Id),
                    StudentDetail = Student.StudentDetail
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
