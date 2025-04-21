using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Variables.Commands.CreatePollVariable;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Students.Commands.CreateStudentCohort
{
    internal class CreateStudentCohortCommandHandler : IRequestHandler<CreateStudentCohortCommand, CreateCommandResponse<Student>>
    {
        private readonly IStudentCohortRepository _studentCohortRepository;
        private readonly ILogger<CreateStudentCohortCommandHandler> _logger;

        public CreateStudentCohortCommandHandler(
            IStudentCohortRepository studentCohortRepository,
            ILogger<CreateStudentCohortCommandHandler> logger)
        {
            _studentCohortRepository = studentCohortRepository;
            _logger = logger;
        }
        public async Task<CreateCommandResponse<Student>> Handle(CreateStudentCohortCommand request, CancellationToken cancellationToken)
        {

            try
            {

                Student student = await _studentCohortRepository.GetByCohortIdAndStudentIdAsync(request.CohortId, request.StudentId);
                if (student != null) return new CreateCommandResponse<Student>(student, 0, "Success", true);


                Student studentCohortToCreate = new Student();
                studentCohortToCreate.CohortId = request.CohortId;
                studentCohortToCreate.Id = request.StudentId;

                Student createdStudentCohort = await _studentCohortRepository.AddAsync(studentCohortToCreate);

                return new CreateCommandResponse<Student>(createdStudentCohort, 1, "Success", true);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating relationship between student and cohort ");
                return new CreateCommandResponse<Student>(null, 0, "Error", false);
            }
        }

    }
}
