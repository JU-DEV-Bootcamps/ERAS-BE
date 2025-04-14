using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries;

public class PollTopHandler(IPollInstanceRepository PollInstanceRepository, ILogger<PollAvgQuery> Logger) : IRequestHandler<PollTopQuery, BaseResponse>
{
    private readonly IPollInstanceRepository _pollInstanceRepository = PollInstanceRepository;
    private readonly ILogger<PollAvgQuery> _logger = Logger;

    public async Task<BaseResponse> Handle(PollTopQuery Req, CancellationToken CancelToken)
    {
        try
        {
            List<Answer> answers  = [];
            Guid pollUuid = Req.PollUuid;
            //Need to construct a response with a list of components, their quetions-answers and the top risk answers per question with the student list with that top risk answer
            //TODO: Consider if instead of only the highest risk. This should return also an average filtering answers above 3 values of risk
            PollInstance allFromPoll = await _pollInstanceRepository.GetAllByPollUuidAsync(pollUuid) ?? throw new KeyNotFoundException("Poll not found");
            //double avgRiskOfAnswers = answers.Average(a => a.RiskLevel);
            return new BaseResponse("The average risk of the selected questions are avgRiskOfAnswers", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while calculating the average risk by student/answer");
            return new BaseResponse(false);
        }
    }
}
