using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries;

public class GetCountSummaryQueryHandler(
    ILogger<GetCountSummaryQueryHandler> Logger,
    IStudentRepository StudentRepository,
    ICohortRepository CohortRepository,
    IEvaluationRepository EvaluationRepository,
    IPollRepository PollRepository,
    IPollInstanceRepository PollInstanceRepository
  ) : IRequestHandler<GetCountSummaryQuery, GetQueryResponse<Dictionary<string, int>>>
{
    private readonly ILogger<GetCountSummaryQueryHandler> _logger = Logger;

    public async Task<GetQueryResponse<Dictionary<string, int>>> Handle(GetCountSummaryQuery Request, CancellationToken CancellationToken)
    {
        var counts = new Dictionary<string, int>();
        try
        {
            counts["Students"] = await StudentRepository.CountAsync();
            counts["Cohorts"] = await CohortRepository.CountAsync();
            counts["Evaluations"] = await EvaluationRepository.CountAsync();
            counts["Polls"] = await PollRepository.CountAsync();
            counts["PollInstances"] = await PollInstanceRepository.CountAsync();

            return new GetQueryResponse<Dictionary<string, int>>(counts, "Counts updated", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong while counting entities.");
            return new GetQueryResponse<Dictionary<string, int>>([], $"Error: {e.Message}", false);
        }
    }
}
