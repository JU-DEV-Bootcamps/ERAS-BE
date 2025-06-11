using MediatR;
using Eras.Application.DTOs.Student;
using Eras.Application.Utils;

namespace Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohorAndPoll
{
    public sealed record GetAllAverageRiskByCohortAndPollQuery(Pagination Pagination, List<int> CohortIds, string PollUuid)
        : IRequest<PagedResult<StudentAverageRiskDto>>;
}
