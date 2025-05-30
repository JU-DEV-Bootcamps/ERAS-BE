using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Cohorts.Commands.CreateCohort
{
    public class CreateCohortCommand : IRequest<CreateCommandResponse<Cohort>>
    {
        public required CohortDTO CohortDto;
    }
}
