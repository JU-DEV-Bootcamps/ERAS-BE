using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Components.Queries.GetByNameAndPoll;
public class GetComponentByNameAndPollIdQuery : IRequest<GetQueryResponse<Component>>
{
    public required string ComponentName;
    public required int PollId;
}
