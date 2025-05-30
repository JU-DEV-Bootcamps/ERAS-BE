using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Variables.Queries.GetByNameAndPollId;
public class GetVariableByNameAndPollIdQueryHandler: 
    IRequestHandler<GetVariableByNameAndPollIdQuery, GetQueryResponse<Variable>>
{
    private IVariableRepository _variableRepository;
    private ILogger<GetVariableByNameAndPollIdQueryHandler> _logger;
    public GetVariableByNameAndPollIdQueryHandler(IVariableRepository VariableRepository, 
        ILogger<GetVariableByNameAndPollIdQueryHandler> Logger) {
        _variableRepository = VariableRepository;
        _logger = Logger;
    }

    public async Task<GetQueryResponse<Variable>> Handle(GetVariableByNameAndPollIdQuery Request, 
        CancellationToken CancellationToken)
    {
        var query = await _variableRepository.GetByNameAndPollIdAsync(Request.VariableName, Request.PollId);
        if (query == null) 
            return new GetQueryResponse<Variable>(new Variable(),"Variable Not Found",true,
                Models.Enums.QueryEnums.QueryResultStatus.NotFound);
        return new GetQueryResponse<Variable>(query, "Variable Found", true);
    }
}
