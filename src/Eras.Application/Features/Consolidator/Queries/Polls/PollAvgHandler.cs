using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries;

public class PollAvgHandler(ILogger<PollAvgHandler> Logger, IPollInstanceRepository PollInstanceRepository) : IRequestHandler<PollAvgQuery, BaseResponse>
{
    private readonly IPollInstanceRepository _pollInstanceRepository = PollInstanceRepository;
    private readonly ILogger<PollAvgHandler> _logger = Logger;

    public async Task<BaseResponse> Handle(PollAvgQuery Req, CancellationToken CancToken)
    {
        try
        {
            Guid pollUuid = Req.PollUuid;
            //Need to construct a response with a list of components, their quetions-answers and the average risk level
            PollInstance allFromPoll = await _pollInstanceRepository.GetAllByPollUuidAsync(pollUuid) ?? throw new KeyNotFoundException("Poll not found");


            //var averageRisk = answers.Average(a => a.RiskLevel);
            return new BaseResponse(allFromPoll.ToString() ?? "No results found", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while calculating the average risk by poll");
            return new BaseResponse(false);
        }
    }
}
