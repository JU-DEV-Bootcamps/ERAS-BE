using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Eras.Application.Mappers;

namespace Eras.Application.Features.Students.Commands.UpdateStudent
{
    internal class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, CreateCommandResponse<Student>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<CreateStudentCommandHandler> _logger;

        public UpdateStudentCommandHandler(IStudentRepository StudentRepository, ILogger<CreateStudentCommandHandler> Logger)
        {
            _studentRepository = StudentRepository;
            _logger = Logger;
        }


        public async Task<CreateCommandResponse<Student>> Handle(UpdateStudentCommand Request, CancellationToken CancellationToken)
        {

            try
            {
                Student? studentDB = await _studentRepository.GetByEmailAsync(Request.StudentDTO.Email);
                if (studentDB == null) return new CreateCommandResponse<Student>(null, 0, "Student Not Found", false);

                Student student = Request.StudentDTO.ToDomain();
                Student studentUpdated = await _studentRepository.UpdateAsync(student);
                return new CreateCommandResponse<Student>(studentUpdated, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred updating student {Request.StudentDTO.Uuid}", ex.Message);
                return new CreateCommandResponse<Student>(null, 0, "Error", false);
            }
        }
    }
}
