using System;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.PollVersions.Queries.GetByPollAndVersion;
public class GetPollVersionByPollAndVersionQuery: IRequest<GetQueryResponse<PollVersion>>
{
    public required string VersionName { get; set; }
    public required int PollId { get; set; }
}
