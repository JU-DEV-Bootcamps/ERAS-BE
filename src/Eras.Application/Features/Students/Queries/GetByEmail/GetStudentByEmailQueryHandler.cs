using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.Student;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Students.Queries.GetByEmail
{
    public class GetStudentByEmailQueryHandler
        : IRequestHandler<GetStudentByEmailQuery, GetQueryResponse<Student>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<GetStudentByEmailQueryHandler> _logger;

        public GetStudentByEmailQueryHandler(IStudentRepository StudentRepository, ILogger<GetStudentByEmailQueryHandler> Logger)
        {
            _studentRepository = StudentRepository;
            _logger = Logger;
        }

        public async Task<GetQueryResponse<Student>> Handle(GetStudentByEmailQuery Request, CancellationToken CancellationToken)
        {
            try
            {
                Student? student = await _studentRepository.GetByEmailAsync(Request.studentEmail);
                if (student == null) {
                    return new GetQueryResponse<Student>(new Student(), "Student dont exist", false);
                }
                return new GetQueryResponse<Student>(student, "Success", true);
            }
            catch (Exception ex) {
                return new GetQueryResponse<Student>(new Student(), "Unexpected error getting the user by email", false);
            }

        }
    }
}
