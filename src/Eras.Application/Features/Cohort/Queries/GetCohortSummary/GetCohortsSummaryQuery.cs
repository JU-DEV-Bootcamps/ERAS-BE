using MediatR;
using Eras.Domain.Entities;
namespace Eras.Application.Features.Cohorts.Queries
{
    public class GetCohortsSummaryQuery: IRequest<List<(Student Student, List<PollInstance> PollInstances)>>;
}