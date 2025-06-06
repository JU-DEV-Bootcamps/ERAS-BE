using Eras.Application.DTOs.Student;

using MediatR;

namespace Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohorAndPoll
{
    public sealed record GetAllAverageRiskByCohortsAndPollQuery(List<int> cohortIds, string PollUuid)
        : IRequest<List<StudentAverageRiskDto>>;
}
