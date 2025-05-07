using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Models.Response.Calculations;

using MediatR;

namespace Eras.Application.Features.Cohort.Queries.GetCohortComponentsByPoll;
public class GetCohortComponentsByPollQuery : IRequest<List<GetCohortComponentsByPollResponse>>
{
    public required string PollUuid { get; set; }
}
