using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.PollInstances.Queries.GetByUuidAndStudentId;
public class GetPollInstanceByUuidAndStudentIdQuery: IRequest<GetQueryResponse<PollInstance>>
{
    public required string PollUuid;
    public required int StudentId;
}
