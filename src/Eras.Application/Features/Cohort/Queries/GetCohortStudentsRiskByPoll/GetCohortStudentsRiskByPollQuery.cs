using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Models.Response.Calculations;

using MediatR;

namespace Eras.Application.Features.Cohort.Queries.GetCohortStudentsRiskByPoll;
public class GetCohortStudentsRiskByPollQuery : IRequest<List<GetCohortStudentsRiskByPollResponse>>
{
    public required string PollUuid { get; set; }
    public required int CohortId { get; set; }
}
