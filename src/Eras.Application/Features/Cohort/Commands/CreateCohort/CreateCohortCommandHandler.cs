using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.StudentsDetails.Commands.CreateStudentDetail;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Cohort.Commands.CreateCohort
{
    public class CreateCohortCommandHandler : IRequestHandler<CreateCohortCommand, CreateComandResponse<Domain.Entities.Cohort>>
    {
        private readonly ICohortRepository _cohortRepository;
        private readonly ILogger<CreateCohortCommandHandler> _logger;

        public CreateCohortCommandHandler(ICohortRepository cohortRepository, ILogger<CreateCohortCommandHandler> logger)
        {
            _cohortRepository = cohortRepository;
            _logger = logger;
        }


        public async Task<CreateComandResponse<Domain.Entities.Cohort>> Handle(CreateCohortCommand request, CancellationToken cancellationToken)
        {

            try
            {
                Domain.Entities.Cohort cohort = request.CohortDto.ToDomain();
                cohort.Audit = new AuditInfo()
                {
                    CreatedBy = "Cosmic latte import",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };
                Domain.Entities.Cohort cohortCreated = await _cohortRepository.AddAsync(cohort);               
                return new CreateComandResponse<Domain.Entities.Cohort>(cohortCreated, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred creating cohort {request.CohortDto}: {ex.Message}");
                return new CreateComandResponse<Domain.Entities.Cohort>(null, 0, "Error", false);
            }
        }
    }
}