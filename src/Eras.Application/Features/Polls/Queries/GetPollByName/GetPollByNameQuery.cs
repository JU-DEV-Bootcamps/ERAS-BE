using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Polls.Queries.GetPollByName;
public class GetPollByNameQuery: IRequest<GetQueryResponse<Poll>>
{
    public required string pollName;
}
