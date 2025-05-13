using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Calculations;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohort.Queries.GetCohortTopRiskStudents;
public class GetCohortTopRiskStudentsQueryHandler : IRequestHandler<GetCohortTopRiskStudentsQuery, List<GetCohortTopRiskStudentsByComponentResponse>>
{
    private readonly ICohortRepository _cohortRepository;
    private readonly ILogger<GetCohortTopRiskStudentsQueryHandler> _logger;
    public GetCohortTopRiskStudentsQueryHandler(ICohortRepository CohortRepository, ILogger<GetCohortTopRiskStudentsQueryHandler> Logger)
    {
        _cohortRepository = CohortRepository;
        _logger = Logger;
    }

    public async Task<List<GetCohortTopRiskStudentsByComponentResponse>> Handle(GetCohortTopRiskStudentsQuery Req, CancellationToken CancellationToken)
    {
        var studentsList = await _cohortRepository.GetCohortTopRiskStudentsAsync(Req.PollUuid, Req.CohortId);
        return studentsList;
    }
}
