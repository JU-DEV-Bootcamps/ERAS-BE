using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Cohorts.Queries
{
    public class GetCohortsListQuery : IRequest<GetQueryResponse<List<Cohort>>>
    {
        public string PollUuid { get; set; } = string.Empty;
        public bool LastVersion { get; set; } = true;
        /// <summary>When set, cohorts of every poll of that evaluation are returned and PollUuid is ignored.</summary>
        public int? EvaluationId { get; set; }
    }
}
