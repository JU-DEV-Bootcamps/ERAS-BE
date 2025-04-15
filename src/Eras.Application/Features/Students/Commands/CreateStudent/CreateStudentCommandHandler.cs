using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, CreateCommandResponse<Student?>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<CreateStudentCommandHandler> _logger;

        public CreateStudentCommandHandler(IStudentRepository StudentRepository, ILogger<CreateStudentCommandHandler> Logger)
        {
            _studentRepository = StudentRepository;
            _logger = Logger;
        }


        public async Task<CreateCommandResponse<Student?>> Handle(CreateStudentCommand Request, CancellationToken CancellationToken)
        {

            try
            {
                Student? studentDB = await _studentRepository.GetByEmailAsync(Request.StudentDTO.Email);
                if (studentDB != null) return new CreateCommandResponse<Student?>(null, 0, "Student Already Exist", false);

                Student student = Request.StudentDTO.ToDomain();
                Student studentCreated = await _studentRepository.AddAsync(student);
                return new CreateCommandResponse<Student?>(studentCreated,1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred importing student {Request.StudentDTO.Uuid}");
                return new CreateCommandResponse<Student?>(null,0, "Error", false);
            }
        }
    }
}
