using Eras.Application.DTOs.Poll;

using MediatR;

namespace Eras.Application.Features.Polls.Queries.GetAllByPollAndCohort
{
    public sealed record GetAllByPollAndCohortQuery(int cohortId, int pollId)
        : IRequest<List<PollVariableDto>>;
}
