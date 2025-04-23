using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Variables.Queries.GetVariablesByPollUuidAndComponent
{
    public class GetVariablesByPollUuidAndComponentQueryHandler
        : IRequestHandler<GetVariablesByPollUuidAndComponentQuery, List<Variable>>
    {
        private readonly IVariableRepository _variableRepository;
        private readonly ILogger<GetVariablesByPollUuidAndComponentQueryHandler> _logger;

        public GetVariablesByPollUuidAndComponentQueryHandler(
            IVariableRepository variableRepository,
            ILogger<GetVariablesByPollUuidAndComponentQueryHandler> logger
        )
        {
            _variableRepository = variableRepository;
            _logger = logger;
        }

        public async Task<List<Variable>> Handle(
            GetVariablesByPollUuidAndComponentQuery request,
            CancellationToken cancellationToken
        )
        {
            var variables = await _variableRepository.GetAllByPollUuidAsync(
                request.pollUuid,
                request.component
            );
            return variables;
        }
    }
}
