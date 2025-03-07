using MediatR;
using Eras.Domain.Entities;
namespace Eras.Application.Features.Cohort.Queries
{
    public class GetCohortsSummaryQuery: IRequest<List<(Student Student, List<PollInstance> PollInstances)>>;
}