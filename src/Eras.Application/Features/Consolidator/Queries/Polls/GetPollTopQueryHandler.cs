using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Views;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class GetPollTopQueryHandler(
  ILogger<GetPollTopQueryHandler> Logger,
  IPollVariableRepository PollVariableRepository
) : IRequestHandler<GetPollTopQuery, List<ErasCalculationsByPollDTO>?>
{
    private readonly ILogger<GetPollTopQueryHandler> _logger = Logger;
    private readonly IPollVariableRepository _pollVariableRepository = PollVariableRepository;
    public async Task<List<ErasCalculationsByPollDTO>?> Handle(GetPollTopQuery Request, CancellationToken CancellationToken)
    {
        var result = await _pollVariableRepository.GetByPollUuidVariableIdAsync(Request.PollUuid.ToString(), Request.VariableIds);
        return result;
    }

}
