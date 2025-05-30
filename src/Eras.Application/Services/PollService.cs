using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

namespace Eras.Application.Services
{
    public class PollService : IPollService
    {

        private readonly IPollRepository _pollRepository;
        public PollService(IPollRepository PollRepository)
        {
            _pollRepository = PollRepository;
        }
        public async Task<Poll> CreatePoll(Poll Poll)
        {
            try
            {
                // we need to check bussiness logic to validate before save
                return await _pollRepository.AddAsync(Poll);

            }
            catch (Exception e)
            {
                // todo pending custom exepcion? disscuss with team
                throw new NotImplementedException($"Error creating poll: {e.Message}");
            }
        }
        public async Task<Poll?> GetPollById(int PollId)
        {
            return await _pollRepository.GetByIdAsync(PollId);
        }
    }
}
