using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Infrastructure
{
    public interface IPollService
    {
        public Task<Poll> CreatePoll(Poll poll);
        public Task<Poll?> GetPollById(int pollId);
    }
}
