using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Consolidator;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class GetRiskCountQueryHandler(IPollVariableRepository PollVariableRepository, ILogger<GetRiskCountQueryHandler> Logger) : IRequestHandler<GetRiskCountQuery, GetQueryResponse<RiskCountResponseVm>>
{
    private readonly ILogger<GetRiskCountQueryHandler> _logger = Logger;
    private readonly IPollVariableRepository _pollVariableRepository = PollVariableRepository;

    public async Task<GetQueryResponse<RiskCountResponseVm>> Handle(
        GetRiskCountQuery Request,
        CancellationToken CancellationToken)
    {
        try
        {
            var maxRiskLevel = 5;
            List<Answer> results = await _pollVariableRepository.GetAnswersByPollUuidAsync(Request.PollUuid.ToString());
            var res = new RiskCountResponseVm()
            {
                AnswerCount = results.Count,
                AverageRisk = (decimal) Math.Round(results.Select(A => A.RiskLevel).Average(),2),
                Risks = [..results
                    .GroupBy(A =>A.RiskLevel >= maxRiskLevel
                        ? maxRiskLevel - 1
                        //TODO: Remove Decimal cast once averages are included in DB
                        : (int) Math.Floor((decimal)A.RiskLevel))
                    .Select(AGroup =>
                    {
                        var end = AGroup.Key == (maxRiskLevel - 1) ? maxRiskLevel : AGroup.Key + 1;
                        return new RiskRange{
                            Label = AGroup.Key == (maxRiskLevel - 1)
                                ? $"Risk {AGroup.Key}+"
                                : $"Risk {AGroup.Key} - {end}",
                            StartRange = AGroup.Key,
                            EndRange = end,
                            Count = AGroup.Count(),
                        };
                    })]
            };
            return new GetQueryResponse<RiskCountResponseVm>(res, "Success", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while counting risks by poll: ");
            return new GetQueryResponse<RiskCountResponseVm>(
                new RiskCountResponseVm() { AverageRisk = 0},
                $"Failed to retrieve risk count by poll. Error {e.Message}",
                false
            );
        }
    }
}
