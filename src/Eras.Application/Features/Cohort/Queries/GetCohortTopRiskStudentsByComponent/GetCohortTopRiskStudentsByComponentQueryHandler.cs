using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Calculations;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohort.Queries.GetCohortTopRiskStudentsByComponent;
public class GetCohortTopRiskStudentsByComponentQueryHandler : IRequestHandler<GetCohortTopRiskStudentsByComponentQuery, List<GetCohortTopRiskStudentsByComponentResponse>>
{
    private readonly ICohortRepository _cohortRepository;
    private readonly ILogger<GetCohortTopRiskStudentsByComponentQueryHandler> _logger;
    public GetCohortTopRiskStudentsByComponentQueryHandler(ICohortRepository CohortRepository, ILogger<GetCohortTopRiskStudentsByComponentQueryHandler> Logger)
    {
        _cohortRepository = CohortRepository;
        _logger = Logger;
    }

    public async Task<List<GetCohortTopRiskStudentsByComponentResponse>> Handle(GetCohortTopRiskStudentsByComponentQuery request, CancellationToken cancellationToken)
    {
        var listStudents = await _cohortRepository.GetCohortTopRiskStudentsByComponent(request.PollUuid, request.ComponentName, request.CohortId);
        return listStudents;
    }
}
