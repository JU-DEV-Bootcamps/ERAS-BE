using Eras.Application.Models.Response.Controllers.CohortsController;
using Eras.Application.Utils;

using MediatR;
namespace Eras.Application.Features.Cohorts.Queries
{
    public class GetCohortsSummaryQuery: IRequest<CohortSummaryResponse>
    {
        public required Pagination Pagination { get; set; }
    }
}