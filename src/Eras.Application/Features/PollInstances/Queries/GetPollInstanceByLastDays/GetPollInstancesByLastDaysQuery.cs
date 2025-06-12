using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.PollInstances.Queries.GetPollInstanceByLastDays
{
    public class GetPollInstancesByLastDaysQuery : IRequest<GetQueryResponse<List<PollInstance>>>
    {
        public int LastDays;
        public bool LastVersion;
        public string PollUuid = String.Empty;
    }
}
