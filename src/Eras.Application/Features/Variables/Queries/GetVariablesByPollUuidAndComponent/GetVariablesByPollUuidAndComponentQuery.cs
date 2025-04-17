using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Variables.Queries.GetVariablesByPollUuidAndComponent
{
    public sealed record GetVariablesByPollUuidAndComponentQuery(
        string pollUuid,
        List<string> component
    ) : IRequest<List<Variable>>;
}
