using Eras.Application.DTOs.Student;
using Eras.Application.Utils;

using MediatR;

namespace Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohortAndPoll
{
    public sealed record GetAllAverageRiskByCohortAndPollQuery(Pagination Pagination, List<int> CohortIds, string PollUuid, bool LastVersion, int? evaluationId)
        : IRequest<PagedResult<StudentAverageRiskDto>>;
}
