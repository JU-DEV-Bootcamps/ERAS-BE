using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Views;
using Eras.Application.Models.Response.Common;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class GetPollTopQueryHandler(
 ILogger<GetPollTopQueryHandler> Logger,
 IPollVariableRepository PollVariableRepository
) : IRequestHandler<GetPollTopQuery, GetQueryResponse<List<ErasCalculationsByPollDTO>?>>
{
    private readonly ILogger<GetPollTopQueryHandler> _logger = Logger;
    private readonly IPollVariableRepository _pollVariableRepository = PollVariableRepository;
    // private readonly int _DefaultTakeNumber = 9999;
    public async Task<GetQueryResponse<List<ErasCalculationsByPollDTO>?>> Handle(GetPollTopQuery Request, CancellationToken CancellationToken)
    {
        var result = await _pollVariableRepository.GetByPollUuidVariableIdAsync(Request.PollUuid.ToString(), Request.VariableIds);
        return new GetQueryResponse<List<ErasCalculationsByPollDTO>?>(
            result,
            "Success",
            true
        );
    }

}
