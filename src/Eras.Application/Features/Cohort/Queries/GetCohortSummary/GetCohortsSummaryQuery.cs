using Eras.Domain.Entities;

using MediatR;
namespace Eras.Application.Features.Cohort.Queries
{
    public class GetCohortsSummaryQuery : IRequest<List<(Student Student, List<PollInstance> PollInstances)>>;
}
