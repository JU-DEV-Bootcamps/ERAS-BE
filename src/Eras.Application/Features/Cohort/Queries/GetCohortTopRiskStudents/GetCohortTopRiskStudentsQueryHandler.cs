using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Calculations;
using Eras.Application.Models.Response.Common;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohorts.Queries.GetCohortTopRiskStudents;
public class GetCohortTopRiskStudentsQueryHandler : IRequestHandler<GetCohortTopRiskStudentsQuery, GetQueryResponse<List<GetCohortTopRiskStudentsByComponentResponse>>>
{
    private readonly ICohortRepository _cohortRepository;
    private readonly ILogger<GetCohortTopRiskStudentsQueryHandler> _logger;
    public GetCohortTopRiskStudentsQueryHandler(ICohortRepository CohortRepository, ILogger<GetCohortTopRiskStudentsQueryHandler> Logger)
    {
        _cohortRepository = CohortRepository;
        _logger = Logger;
    }

    public async Task<GetQueryResponse<List<GetCohortTopRiskStudentsByComponentResponse>>> Handle(GetCohortTopRiskStudentsQuery Request, CancellationToken CancellationToken)
    {
        var studentsList = await _cohortRepository.GetCohortTopRiskStudentsAsync(Request.PollUuid, Request.CohortId);
        return new GetQueryResponse<List<GetCohortTopRiskStudentsByComponentResponse>>(studentsList);
    }
}
