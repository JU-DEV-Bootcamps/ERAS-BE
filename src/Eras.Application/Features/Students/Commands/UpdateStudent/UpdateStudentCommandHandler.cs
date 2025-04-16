﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public UpdateStudentCommandHandler(IStudentRepository studentRepository, ILogger<CreateStudentCommandHandler> logger)
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }


        public async Task<CreateCommandResponse<Student>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {

            try
            {
                Student? studentDB = await _studentRepository.GetByEmailAsync(request.StudentDTO.Email);
                if (studentDB == null) return new CreateCommandResponse<Student>(null, 0, "Student Not Found", false);

                Student student = request.StudentDTO.ToDomain();
                Student studentUpdated = await _studentRepository.UpdateAsync(student);
                return new CreateCommandResponse<Student>(studentUpdated, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred updating student {request.StudentDTO.Uuid}");
                return new CreateCommandResponse<Student>(null, 0, "Error", false);
            }
        }
    }
}
