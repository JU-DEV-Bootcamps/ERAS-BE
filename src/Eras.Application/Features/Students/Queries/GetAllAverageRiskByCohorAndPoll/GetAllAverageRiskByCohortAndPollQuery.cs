using Eras.Application.DTOs.Student;

using MediatR;

namespace Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohorAndPoll
{
    public sealed record GetAllAverageRiskByCohortAndPollQuery(int cohortId, int pollId)
        : IRequest<List<StudentAverageRiskDto>>;
}
