using MediatR;
namespace Eras.Application.Features.Cohort.Queries
{
    public class GetCohortsSummaryQuery: IRequest<List<Eras.Domain.Entities.Cohort>>;
}
