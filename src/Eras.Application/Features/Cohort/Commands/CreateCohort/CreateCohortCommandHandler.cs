using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohorts.Commands.CreateCohort
{
    public class CreateCohortCommandHandler : IRequestHandler<CreateCohortCommand, CreateCommandResponse<Domain.Entities.Cohort>>
    {
        private readonly ICohortRepository _cohortRepository;
        private readonly ILogger<CreateCohortCommandHandler> _logger;

        public CreateCohortCommandHandler(ICohortRepository CohortRepository, ILogger<CreateCohortCommandHandler> Logger)
        {
            _cohortRepository = CohortRepository;
            _logger = Logger;
        }


        public async Task<CreateCommandResponse<Domain.Entities.Cohort>> Handle(CreateCohortCommand Request, CancellationToken CancellationToken)
        {

            try
            {
                Domain.Entities.Cohort? cohort = await _cohortRepository.GetByNameAsync(Request.CohortDto.Name);
                if (cohort != null)
                    return new CreateCommandResponse<Domain.Entities.Cohort>(cohort, 0, "Success", true);
                Domain.Entities.Cohort cohortToCreate = Request.CohortDto.ToDomain();
                Domain.Entities.Cohort cohortCreated = await _cohortRepository.AddAsync(cohortToCreate);
                return new CreateCommandResponse<Domain.Entities.Cohort>(cohortCreated, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred creating cohort {Request.CohortDto}: {ex.Message}");
                return new CreateCommandResponse<Domain.Entities.Cohort>(new Domain.Entities.Cohort(), 0, "Error", false);
            }
        }
    }
}
