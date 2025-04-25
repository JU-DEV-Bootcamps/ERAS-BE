using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Cohort.Commands.CreateCohort
{
    public class CreateCohortCommand : IRequest<CreateCommandResponse<Domain.Entities.Cohort>>
    {
        public required CohortDTO CohortDto;
    }
}
