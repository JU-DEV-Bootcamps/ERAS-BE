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
using Eras.Domain.Entities;
using Eras.Application.DTOs;
using Eras.Domain.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Eras.Application.Features.StudentsDetails.Commands.CreateStudentDetail;
using Eras.Application.Models.Response.Common;
using Eras.Application.Features.Students.Queries.GetByEmail;
using Eras.Application.Features.Students.Commands.UpdateStudent;
using Eras.Domain.Common.Exceptions;

namespace Eras.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentsCommandHandler : IRequestHandler<CreateStudentsCommand, CreateCommandResponse<Student[]>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<CreateStudentsCommandHandler> _logger;
        private readonly IMediator _mediator;

        public CreateStudentsCommandHandler(IStudentRepository StudentRepository, ILogger<CreateStudentsCommandHandler> Logger, IMediator Mediator)
        {
            _studentRepository = StudentRepository;
            _logger = Logger;
            _mediator = Mediator;
        }

        public async Task<CreateCommandResponse<Student[]>> Handle(CreateStudentsCommand Request, CancellationToken CancellationToken)
        {
            try
            {
                _logger.LogInformation("Importing students");
                List<Student> createdStudents = [];
                List<Student> updatedStudents = [];
                List<Student> errorStudents = [];

                foreach (StudentImportDto dto in Request.students)
                {
                    StudentDTO studentDTO = dto.ExtractStudentDTO();
                    studentDTO.IsImported = true;
                    studentDTO.Audit = new AuditInfo()
                    {
                        CreatedBy = "CSV import",
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow,
                    };
                    GetStudentByEmailQuery getStudentByEmailQuery = new GetStudentByEmailQuery() { studentEmail = studentDTO.Email };
                    CreateCommandResponse<Student> studentCreatedOrChanged;
                    try
                    {
                        GetQueryResponse<Student> getStudentResponse = await _mediator.Send(getStudentByEmailQuery);
                        UpdateStudentCommand updateStudentCommand = new UpdateStudentCommand() { StudentDTO = studentDTO };
                        studentCreatedOrChanged = await _mediator.Send(updateStudentCommand);
                    }
                    catch (EntityNotFoundException Ex) {
                        CreateStudentCommand createStudentCommand = new CreateStudentCommand() { StudentDTO = studentDTO };
                        studentCreatedOrChanged = await _mediator.Send(createStudentCommand);
                    }
                    if (!studentCreatedOrChanged.Success)
                    {
                        errorStudents.Add(studentCreatedOrChanged.Entity);
                    }
                    else if (studentCreatedOrChanged.Success)
                    {
                        if (studentCreatedOrChanged.SuccessfullImports == 0)
                            updatedStudents.Add(studentCreatedOrChanged.Entity);
                        CreateCommandResponse<StudentDetail> createdStudentDetail = await CreateStudentDetailAsync(studentCreatedOrChanged.Entity, dto);
                        studentCreatedOrChanged.Entity.StudentDetail = createdStudentDetail.Entity;
                        createdStudents.Add(studentCreatedOrChanged.Entity);
                    }
                }
                return new CreateCommandResponse<Student[]>(createdStudents.ToArray(), createdStudents.Count, $"{createdStudents.Count} new students, {updatedStudents.Count} updated, and {errorStudents.Count} with errors.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the massive import process");
                return new CreateCommandResponse<Student[]>([new Student()],0, "Error", false);
            }
        }
        public async Task<CreateCommandResponse<StudentDetail>> CreateStudentDetailAsync(Student Student, StudentImportDto Dto)
        {
            Student.StudentDetail.EnrolledCourses = Dto.EnrolledCourses;
            Student.StudentDetail.GradedCourses = Dto.GradedCourses;
            Student.StudentDetail.TimeDeliveryRate = Dto.TimelySubmissions;
            Student.StudentDetail.AvgScore = Dto.AverageScore;
            Student.StudentDetail.CoursesUnderAvg = Dto.CoursesBelowAverage;
            Student.StudentDetail.PureScoreDiff = Dto.RawScoreDifference;
            Student.StudentDetail.StandardScoreDiff = Dto.StandardScoreDifference;
            Student.StudentDetail.LastAccessDays = Dto.DaysSinceLastAccess;
            Student.StudentDetail.Audit.ModifiedAt = DateTime.UtcNow;
            StudentDetailDTO studentDetailDTO = Student.StudentDetail.ToDto();
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
