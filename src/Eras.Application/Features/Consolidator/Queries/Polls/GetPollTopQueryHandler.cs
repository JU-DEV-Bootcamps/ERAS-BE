using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Views;
using Eras.Application.Utils;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class GetPollTopQueryHandler(
  ILogger<GetPollTopQueryHandler> Logger,
  IPollVariableRepository PollVariableRepository
) : IRequestHandler<GetPollTopQuery, PagedResult<ErasCalculationsByPollDTO>?>
{
    private readonly ILogger<GetPollTopQueryHandler> _logger = Logger;
    private readonly IPollVariableRepository _pollVariableRepository = PollVariableRepository;
    public async Task<PagedResult<ErasCalculationsByPollDTO>?> Handle(GetPollTopQuery Request, CancellationToken CancellationToken)
    {
        var result = await _pollVariableRepository.GetByPollUuidVariableIdAsync(Request.PollUuid.ToString(), Request.VariableIds, Request.Pagination);
        return result;
    }

}
