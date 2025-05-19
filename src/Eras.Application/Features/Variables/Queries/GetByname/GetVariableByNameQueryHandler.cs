using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Variables.Queries.GetByName;
public class GetVariableByNameQueryHandler: 
    IRequestHandler<GetVariableByNameQuery, GetQueryResponse<Variable>>
{
    private IVariableRepository _variableRepository;
    private ILogger<GetVariableByNameQueryHandler> _logger;
    public GetVariableByNameQueryHandler(IVariableRepository VariableRepository, 
        ILogger<GetVariableByNameQueryHandler> Logger) {
        _variableRepository = VariableRepository;
        _logger = Logger;
    }

    public async Task<GetQueryResponse<Variable>> Handle(GetVariableByNameQuery Request, 
        CancellationToken CancellationToken)
    {
        var query = await _variableRepository.GetByNameAsync(Request.VariableName);
        if (query == null) 
            return new GetQueryResponse<Variable>(new Variable(),"Variable Not Found",true,
                Models.Enums.QueryEnums.QueryResultStatus.NotFound);
        return new GetQueryResponse<Variable>(query, "Variable Found", true);
    }
}
