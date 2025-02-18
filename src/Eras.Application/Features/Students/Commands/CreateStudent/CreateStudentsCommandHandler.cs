using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Mappers;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;
using Eras.Application.Models;
using Eras.Domain.Entities;
using Eras.Application.DTOs;
using Eras.Domain.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Eras.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentsCommandHandler : IRequestHandler<CreateStudentsCommand, CreateComandResponse<Student[]>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<CreateStudentsCommandHandler> _logger;
        private readonly IMediator _mediator;

        public CreateStudentsCommandHandler(IStudentRepository studentRepository, ILogger<CreateStudentsCommandHandler> logger, IMediator mediator)
        {
            _studentRepository = studentRepository;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<CreateComandResponse<Student[]>> Handle(CreateStudentsCommand request, CancellationToken cancellationToken)
        {
            try
            { 
                _logger.LogInformation("Importing students");
                List<Student> createdStudents = [];
                List<Student> updatedStudents = [];
                List<Student> errorStudents = [];

                foreach (StudentImportDto dto in request.students)
                {
                    StudentDTO studentDTO = dto.ExtractStudentDTO();
                    studentDTO.Audit = new AuditInfo()
                    {
                        CreatedBy = "CSV import",
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow,
                    };
                    CreateStudentCommand createStudentCommand = new CreateStudentCommand() { StudentDTO = studentDTO };
                    CreateComandResponse<Student> createdStudent = await _mediator.Send(createStudentCommand);

                    if (! createdStudent.Success) {
                        errorStudents.Add(createdStudent.Entity);
                    } else if (createdStudent.Success && createdStudent.SuccessfullImports == 0)
                    {
                        updatedStudents.Add(createdStudent.Entity);
                    }
                    else
                    {
                        createdStudents.Add(createdStudent.Entity);
                    }
                }
                return new CreateComandResponse<Student[]>(createdStudents.ToArray(), createdStudents.Count, $"{createdStudents.Count} new students, {updatedStudents.Count} updated, and {errorStudents.Count} with errors.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the massive import process");
                return new CreateComandResponse<Student[]>(null,0, "Error", false);
            }
        }
    }
}
