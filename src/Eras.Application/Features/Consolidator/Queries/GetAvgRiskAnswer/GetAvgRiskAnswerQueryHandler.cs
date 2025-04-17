using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.GetAvgRiskAnswer;

public class GetAvgRiskAnswerQueryHandler: IRequestHandler<GetAvgRiskAnswerQuery, BaseResponse>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly ILogger<GetAvgRiskAnswerQuery> _logger;

    public GetAvgRiskAnswerQueryHandler(IAnswerRepository answerRepository, ILogger<GetAvgRiskAnswerQuery> logger)
    {
        _answerRepository = answerRepository;
        _logger = logger;
    }

    public async Task<BaseResponse> Handle(GetAvgRiskAnswerQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var studentIds = request.StudentIds;
            var answerIds = request.AnswerIds;
            List<Answer> answers  = [];
            foreach (var answerId in answerIds)
            {
                //TODO: Add repo method to get by student id and answer id
                //var answerDTO = await _answerRepository.GetByStudentIdAndAnswerId(answer.StudentId, answer.AnswerId);
                var answer = await _answerRepository.GetByIdAsync(answerId);
                if(answer != null) {
                    answers.Add(answer);
                }
            }
            //TODO: Implement logic to calculate the average risk answer
            double avgRiskOfAnswers = answers.Average(a => a.RiskLevel);
            return new BaseResponse($"The average risk of the selected questions are {avgRiskOfAnswers}", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while calculating the average risk by student/answer: " + request);
            return new BaseResponse(false);
        }
    }
}
