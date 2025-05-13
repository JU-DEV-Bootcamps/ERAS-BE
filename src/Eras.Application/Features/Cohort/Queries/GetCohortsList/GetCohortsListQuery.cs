using MediatR;

namespace Eras.Application.Features.Cohort.Queries
{
    public class GetCohortsListQuery : IRequest<List<Domain.Entities.Cohort>>
    {
        public string PollUuid { get; set; } = string.Empty;
    }
}
