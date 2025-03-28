﻿using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, CreateCommandResponse<Student>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<CreateStudentCommandHandler> _logger;

        public CreateStudentCommandHandler(IStudentRepository studentRepository, ILogger<CreateStudentCommandHandler> logger)
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }


        public async Task<CreateCommandResponse<Student>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {

            try
            {
                Student? studentDB = await _studentRepository.GetByEmailAsync(request.StudentDTO.Email);
                if (studentDB != null) return new CreateCommandResponse<Student>(studentDB, 0, "Success", true);

                Student student = request.StudentDTO.ToDomain();
                Student studentCreated = await _studentRepository.AddAsync(student);
                return new CreateCommandResponse<Student>(studentCreated,1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred importing student {request.StudentDTO.Uuid}");
                return new CreateCommandResponse<Student>(null,0, "Error", false);
            }
        }
    }
}
