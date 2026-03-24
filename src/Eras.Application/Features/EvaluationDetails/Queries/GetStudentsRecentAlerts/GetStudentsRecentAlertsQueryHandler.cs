using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;
using Eras.Application.Models.Response.Controllers.StudentsController;
using Eras.Application.Utils;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.EvaluationDetails.Queries.GetStudentsRecentAlerts;

public class GetStudentsRecentAlertsQueryHandler
    : IRequestHandler<GetStudentsRecentAlertsQuery, PagedResult<GetStudentsRecentAlertsResponse>>
{
    private readonly IErasEvaluationDetailsViewRepository _viewRepository;
    private readonly ILogger<GetStudentsRecentAlertsQueryHandler> _logger;

    public GetStudentsRecentAlertsQueryHandler(
        IErasEvaluationDetailsViewRepository viewRepository,
        ILogger<GetStudentsRecentAlertsQueryHandler> logger
    )
    {
        _viewRepository = viewRepository;
        _logger = logger;
    }

    public async Task<PagedResult<GetStudentsRecentAlertsResponse>> Handle(
        GetStudentsRecentAlertsQuery Request, 
        CancellationToken CancellationToken)
    {
        try
        {
            var alerts = await _viewRepository.GetRecentAlertsStudentAsync(
                Request.Query.Page,
                Request.Query.PageSize
            );
            var totalCount = await _viewRepository.CountAsync();

            PagedResult<GetStudentsRecentAlertsResponse> pagedResult = new PagedResult<GetStudentsRecentAlertsResponse>(
                totalCount,
                alerts.ToList()
            );

            return pagedResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting all recent reports: " + ex.Message);
            return new PagedResult<GetStudentsRecentAlertsResponse>(0, []);
        }
    }
}
