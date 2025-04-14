using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.Student;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Students.Queries.GetByEmail
{
    public class GetStudentByEmailQueryHandler
        : IRequestHandler<GetStudentByEmailQuery, GetQueryResponse<Student?>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<GetStudentByEmailQueryHandler> _logger;

        public GetStudentByEmailQueryHandler(IStudentRepository studentRepository, ILogger<GetStudentByEmailQueryHandler> logger)
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }

        public async Task<GetQueryResponse<Student>> Handle(GetStudentByEmailQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Student student = await _studentRepository.GetByEmailAsync(request.studentEmail);
                if (student == null) {
                    return new GetQueryResponse<Student>(null, "Student dont exist", true);
                }
                return new GetQueryResponse<Student>(student, "Success", true);
            }
            catch (Exception ex) {
                return new GetQueryResponse<Student>(null, "Unexpected error getting the user by email", false);
            }

        }
    }
}
