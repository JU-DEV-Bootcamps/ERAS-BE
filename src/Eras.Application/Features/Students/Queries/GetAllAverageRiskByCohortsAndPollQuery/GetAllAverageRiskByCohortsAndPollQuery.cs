using Eras.Application.DTOs.Student;
using Eras.Application.Utils;

using MediatR;

namespace Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohorAndPoll
{
    public sealed record GetAllAverageRiskByCohortsAndPollQuery(Pagination Pagination, List<int> cohortIds, string PollUuid, bool LastVersion)
        : IRequest<PagedResult<StudentAverageRiskDto>>;
}
