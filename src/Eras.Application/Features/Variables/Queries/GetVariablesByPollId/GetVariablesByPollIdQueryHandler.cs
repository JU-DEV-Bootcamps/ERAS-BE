using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Variables.Queries.GetVariablesByPollId
{
    public class GetVariablesByPollIdQueryHandler
        : IRequestHandler<GetVariablesByPollIdAndComponentQuery, List<Variable>>
    {
        private readonly IVariableRepository _variableRepository;
        private readonly ILogger<GetVariablesByPollIdQueryHandler> _logger;

        public GetVariablesByPollIdQueryHandler(
            IVariableRepository variableRepository,
            ILogger<GetVariablesByPollIdQueryHandler> logger
        )
        {
            _variableRepository = variableRepository;
            _logger = logger;
        }

        public async Task<List<Variable>> Handle(
            GetVariablesByPollIdAndComponentQuery request,
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
