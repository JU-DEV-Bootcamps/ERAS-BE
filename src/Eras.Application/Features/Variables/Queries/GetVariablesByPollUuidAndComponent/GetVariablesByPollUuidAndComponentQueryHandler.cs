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
            IVariableRepository VariableRepository,
            ILogger<GetVariablesByPollUuidAndComponentQueryHandler> Logger
        )
        {
            _variableRepository = VariableRepository;
            _logger = Logger;
        }

        public async Task<List<Variable>> Handle(
            GetVariablesByPollUuidAndComponentQuery Request,
            CancellationToken CancellationToken
        )
        {
            var variables = await _variableRepository.GetAllByPollUuidAsync(
                Request.pollUuid,
                Request.component
            );
            return variables;
        }
    }
}
