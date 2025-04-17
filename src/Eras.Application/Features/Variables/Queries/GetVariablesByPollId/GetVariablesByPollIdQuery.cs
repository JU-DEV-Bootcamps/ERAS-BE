using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Variables.Queries.GetVariablesByPollId
{
    public sealed record GetVariablesByPollIdAndComponentQuery(
        string pollUuid,
        List<string> component
    ) : IRequest<List<Variable>>;
}
