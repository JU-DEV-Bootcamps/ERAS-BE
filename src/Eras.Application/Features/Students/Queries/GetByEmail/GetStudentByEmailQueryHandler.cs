using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

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
            Student? student = await _studentRepository.GetByEmailAsync(Request.studentEmail) ?? throw new NotFoundException("Student not found"); ;
            return new GetQueryResponse<Student>(student, "Success", true);
        }
    }
}
