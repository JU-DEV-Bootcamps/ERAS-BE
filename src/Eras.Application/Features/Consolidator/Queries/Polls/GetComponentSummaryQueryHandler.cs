using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class GetComponentSummaryQueryHandler : IRequestHandler<GetComponentSummaryQuery, GetQueryResponse<List<Answer>>>
{
    private readonly ILogger<GetComponentSummaryQueryHandler> _logger;
    private readonly IPollVariableRepository _pollVariableRepository;

    public GetComponentSummaryQueryHandler(IPollVariableRepository PollVariableRepository, ILogger<GetComponentSummaryQueryHandler> Logger)
    {
        _pollVariableRepository = PollVariableRepository;
        _logger = Logger;
    }
    public async Task<GetQueryResponse<List<Answer>>> Handle(
        GetComponentSummaryQuery Request,
        CancellationToken CancellationToken)
    {
        try
        {
            List<Answer> results = await _pollVariableRepository.GetSummaryByPollUuidAsync(Request.PollUuid.ToString());
            return new GetQueryResponse<List<Answer>>(results, "Success", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while calculating higher risk students: ");
            return new GetQueryResponse<List<Answer>>(
                [],
                $"Failed to retrieve top risk students by variable. Error {e.Message}",
                false
            );
        }
    }
}
