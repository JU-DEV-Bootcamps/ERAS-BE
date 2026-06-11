using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Variables.Queries.GetWithNameAndPollId;
public class GetVariablesWithNameAndPollIdQueryHandler: 
    IRequestHandler<GetVariablesWithNameAndPollIdQuery, GetQueryResponse<List<Variable>>>
{
    private IVariableRepository _variableRepository;
    private ILogger<GetVariablesWithNameAndPollIdQueryHandler> _logger;
    public GetVariablesWithNameAndPollIdQueryHandler(IVariableRepository VariableRepository, 
        ILogger<GetVariablesWithNameAndPollIdQueryHandler> Logger) {
        _variableRepository = VariableRepository;
        _logger = Logger;
    }

    public async Task<GetQueryResponse<List<Variable>>> Handle(GetVariablesWithNameAndPollIdQuery Request, 
        CancellationToken CancellationToken)
    {
        List<Variable> result = await _variableRepository.GetAllWithNameAndPollIdAsync();
        if (result.Count == 0) 
            return new GetQueryResponse<List<Variable>>(result,"Variables Not Found",true,
                Models.Enums.QueryEnums.QueryResultStatus.NotFound);
        return new GetQueryResponse<List<Variable>>(result, "Variables Found", true);
    }
}