using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.Cohorts.Commands.CreateCohort;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Features.Students.Commands.CreateStudentCohort;
using Eras.Application.Features.Students.Queries.GetByEmail;
using Eras.Application.Features.StudentsDetails.Commands.CreateStudentDetail;
using Eras.Application.Features.StudentsDetails.Queries.GetStudentDetailByStudentId;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Services
{
    /// <summary>
    /// Imports the student aggregate (student, detail and cohort) referenced by a poll.
    /// </summary>
    public class StudentImporter
    {
        private readonly IMediator _mediator;
        private readonly ILogger<StudentImporter> _logger;

        public StudentImporter(IMediator Mediator, ILogger<StudentImporter> Logger)
        {
            _mediator = Mediator;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<Student>> CreateStudentFromPollAsync(PollDTO PollToCreate)
        {
            try
            {
                StudentDTO studentToCreate = (PollToCreate.Components.FirstOrDefault()?.Variables.FirstOrDefault()?.Answer?.Student)
                    ?? throw new ArgumentNullException("Student information not found");
                studentToCreate.Uuid = Guid.NewGuid().ToString();
                studentToCreate.Audit = new AuditInfo()
                {
                    CreatedBy = "Cosmic Latte Import",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };

                GetStudentByEmailQuery getStudentByEmailQuery = new GetStudentByEmailQuery() { studentEmail = studentToCreate.Email };
                CreateCommandResponse<Student> createdStudent;
                try
                {
                    GetQueryResponse<Student> getStudentResponse = await _mediator.Send(getStudentByEmailQuery);
                    createdStudent = new CreateCommandResponse<Student>(getStudentResponse.Body, "Success", true);
                }
                catch (NotFoundException)
                {
                    CreateStudentCommand createStudentCommand = new CreateStudentCommand() { StudentDTO = studentToCreate };
                    createdStudent = await _mediator.Send(createStudentCommand);
                }
                if (createdStudent.Success && createdStudent.Entity != null)
                {
                    CreateCommandResponse<StudentDetail> createdStudentDetail = await CreateStudentDetailAsync(createdStudent.Entity.Id);
                    CohortDTO? cohortToCreate = studentToCreate.Cohort;

                    if (createdStudentDetail.Entity != null && cohortToCreate != null)
                    {
                        createdStudent.Entity.StudentDetail = createdStudentDetail.Entity;
                        CreateCommandResponse<Cohort> createdCohort = await CreateAndSetStudentCohortAsync(createdStudent.Entity.ToDto(), cohortToCreate);
                        createdStudent.Entity.Cohort = createdCohort.Entity;
                    }
                    else
                    {
                        _logger.LogWarning($"Warning creating student: createdStudentDetail or cohortToCreate is null");
                    }
                }
                return createdStudent;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating student: {ex.Message}");
                return new CreateCommandResponse<Student>(null, 0, "Error", false);
            }
        }

        public async Task<CreateCommandResponse<StudentDetail>> CreateStudentDetailAsync(int StudentId)
        {
            GetStudentDetailByStudentIdQuery query = new GetStudentDetailByStudentIdQuery()
            {
                StudentId = StudentId
            };
            GetQueryResponse<StudentDetail?> createdStudentDetail = await _mediator.Send(query);
            if (createdStudentDetail.Success && createdStudentDetail.Body != null)
            {
                CreateCommandResponse<StudentDetail> command = new CreateCommandResponse<StudentDetail>(createdStudentDetail.Body,
                    0, createdStudentDetail.Message, true);
                return command;
            }
            else
            {
                StudentDetailDTO studentDetailDTO = new StudentDetailDTO() { StudentId = StudentId };
                studentDetailDTO.Audit = new AuditInfo()
                {
                    CreatedBy = "Cosmic latte import",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };
                CreateStudentDetailCommand createStudentDetailCommand = new CreateStudentDetailCommand() { StudentDetailDto = studentDetailDTO };
                return await _mediator.Send(createStudentDetailCommand);
            }
        }

        public async Task<CreateCommandResponse<Cohort>> CreateAndSetStudentCohortAsync(StudentDTO StudentDto, CohortDTO Cohort)
        {
            Cohort.Audit = new AuditInfo()
            {
                CreatedBy = "Cosmic latte import",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            };
            CreateCohortCommand createCohortCommand = new CreateCohortCommand() { CohortDto = Cohort };
            CreateCommandResponse<Cohort> createdCohort = await _mediator.Send(createCohortCommand);

            if (createdCohort.Success && createdCohort.Entity != null)
            {
                CreateStudentCohortCommand createStudentCohortCommand = new CreateStudentCohortCommand()
                {
                    CohortId = createdCohort.Entity.Id,
                    StudentId = StudentDto.Id
                };
                CreateCommandResponse<Student> createdStudentCohort = await _mediator.Send(createStudentCohortCommand);
            }

            return createdCohort;
        }
    }
}
