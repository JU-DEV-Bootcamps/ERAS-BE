using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.PollVersions.Queries.GetAllByPoll;
public class GetAllPollVersionByPollQuery: IRequest<GetQueryResponse<List<PollVersion>>>
{
    public required int PollId;
}
