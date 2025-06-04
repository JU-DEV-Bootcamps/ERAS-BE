using Eras.Application.DTOs.Student;

using MediatR;

namespace Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohorAndPoll
{
    public sealed record GetAllAverageRiskByCohortsAndPollQuery(int[] cohortIds, int PollId)
        : IRequest<List<StudentAverageRiskDto>>;
}
