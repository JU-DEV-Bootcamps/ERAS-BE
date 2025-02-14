using System;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;

public class GetHigherRiskStudentQueryHandler: IRequestHandler<GetHigherRiskStudentQuery, BaseResponse>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly ILogger<GetHigherRiskStudentQueryHandler> _logger;

    public GetHigherRiskStudentQueryHandler(IAnswerRepository answerRepository, ILogger<GetHigherRiskStudentQueryHandler> logger)
    {
        _answerRepository = answerRepository;
        _logger = logger;
    }

    public async Task<BaseResponse> Handle(GetHigherRiskStudentQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return new BaseResponse(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating the Ruleset: " + request);
            return new BaseResponse(false);
        }
    }
}
