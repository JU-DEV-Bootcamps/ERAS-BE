using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Variables.Queries.GetByNameAndPollId;
public class GetVariableByNameAndPollIdQuery: IRequest<GetQueryResponse<Variable>>
{
    public required string VariableName { get; set; }
    public required int PollId { get; set; }
}
