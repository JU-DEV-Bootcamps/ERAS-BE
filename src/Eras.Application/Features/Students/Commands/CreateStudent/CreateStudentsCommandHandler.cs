﻿using System;
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
using Eras.Application.Features.StudentsDetails.Commands.CreateStudentDetail;

namespace Eras.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentsCommandHandler : IRequestHandler<CreateStudentsCommand, CreateCommandResponse<Student[]>>
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

        public async Task<CreateCommandResponse<Student[]>> Handle(CreateStudentsCommand request, CancellationToken cancellationToken)
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
                    CreateCommandResponse<Student> createdStudent = await _mediator.Send(createStudentCommand);

                    if (! createdStudent.Success) {
                        errorStudents.Add(createdStudent.Entity);
                    } else if (createdStudent.Success)
                    {
                        if(createdStudent.SuccessfullImports == 0)
                            updatedStudents.Add(createdStudent.Entity);
                        CreateCommandResponse<StudentDetail> createdStudentDetail = await CreateStudentDetail(createdStudent.Entity,dto);
                        createdStudent.Entity.StudentDetail = createdStudentDetail.Entity;
                        createdStudents.Add(createdStudent.Entity);
                    }
                }
                return new CreateCommandResponse<Student[]>(createdStudents.ToArray(), createdStudents.Count, $"{createdStudents.Count} new students, {updatedStudents.Count} updated, and {errorStudents.Count} with errors.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the massive import process");
                return new CreateCommandResponse<Student[]>(null,0, "Error", false);
            }
        }
        public async Task<CreateCommandResponse<StudentDetail>> CreateStudentDetail(Student student, StudentImportDto dto)
        {
            student.StudentDetail.EnrolledCourses = dto.EnrolledCourses;
            student.StudentDetail.GradedCourses = dto.GradedCourses;
            student.StudentDetail.TimeDeliveryRate = dto.TimelySubmissions;
            student.StudentDetail.AvgScore = dto.AverageScore;
            student.StudentDetail.CoursesUnderAvg = dto.CoursesBelowAverage;
            student.StudentDetail.PureScoreDiff = dto.RawScoreDifference;
            student.StudentDetail.StandardScoreDiff = dto.StandardScoreDifference;
            student.StudentDetail.LastAccessDays = dto.DaysSinceLastAccess;
            student.StudentDetail.Audit.ModifiedAt = DateTime.UtcNow;
            StudentDetailDTO studentDetailDTO = student.StudentDetail.ToDto();
            studentDetailDTO.Audit = new AuditInfo()
            {
                CreatedBy = "Csv latte import",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            };
            CreateStudentDetailCommand createStudentDetailCommand = new CreateStudentDetailCommand() { StudentDetailDto = studentDetailDTO };
            return await _mediator.Send(createStudentDetailCommand);
        }
    }
}
