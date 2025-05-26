using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Components.Queries.GetByNameAndPoll;
public class GetComponentByNameAndPollIdQuery : IRequest<GetQueryResponse<Component>>
{
    public required string ComponentName;
    public required int PollId;
}
