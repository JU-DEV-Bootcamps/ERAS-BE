using Eras.Application.Mappers;
using Eras.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;
using Eras.Domain.Entities;
using Eras.Application.DTOs;
using Eras.Domain.Common;
using Eras.Application.Features.StudentsDetails.Commands.CreateStudentDetail;
using Eras.Application.Models.Response.Common;
using Eras.Application.Features.Students.Queries.GetByEmail;
using Eras.Application.Features.Students.Commands.UpdateStudent;
using Eras.Application.Utils;
using Eras.Error.Bussiness;

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
                    if (StudentValidator.isStudentValid(dto))
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
                            getStudentResponse.Body.IsImported = true;
                            UpdateStudentCommand updateStudentCommand = new UpdateStudentCommand() { StudentDTO = getStudentResponse.Body.ToDto() };
                            studentCreatedOrChanged = await _mediator.Send(updateStudentCommand);
                        }
                        catch (NotFoundException)
                        {
                            CreateStudentCommand createStudentCommand = new CreateStudentCommand() { StudentDTO = studentDTO };
                            studentCreatedOrChanged = await _mediator.Send(createStudentCommand);
                        }

                        if (!studentCreatedOrChanged.Success && studentCreatedOrChanged.Entity != null)
                        {
                            errorStudents.Add(studentCreatedOrChanged.Entity);
                        }
                        else if (studentCreatedOrChanged.Success)
                        {
                            if (studentCreatedOrChanged.SuccessfullImports == 0)
                                updatedStudents.Add(studentCreatedOrChanged.Entity!);

                            CreateCommandResponse<StudentDetail> createdStudentDetail = await CreateStudentDetailAsync(studentCreatedOrChanged.Entity!, dto);
                            studentCreatedOrChanged.Entity!.StudentDetail = createdStudentDetail.Entity!;
                            createdStudents.Add(studentCreatedOrChanged.Entity);
                        }
                    } else
                    {
                        Student errorStudent = new Student
                        {
                            Name = dto.Name,
                            Email = dto.Email,
                        };
                        errorStudents.Add(errorStudent);
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
            Student.Name = Dto.Name;
            Student.Email = Dto.Email;
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
