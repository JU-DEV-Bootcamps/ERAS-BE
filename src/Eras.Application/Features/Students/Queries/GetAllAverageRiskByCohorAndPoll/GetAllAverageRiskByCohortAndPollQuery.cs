using Eras.Application.DTOs.Student;
using MediatR;

namespace Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohorAndPoll
{
    public sealed record GetAllAverageRiskByCohortAndPollQuery(List<int> CohortIds, string PollUuid)
        : IRequest<List<StudentAverageRiskDto>>;
}
