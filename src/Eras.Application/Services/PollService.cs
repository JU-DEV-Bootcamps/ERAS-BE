using Eras.Application.Contracts;
using Eras.Domain.Entities;
using Eras.Domain.Repositories;


namespace Eras.Application.Services
{
    public class PollService : IPollService
    {

        private readonly IPollRepository _pollRepository;
        public PollService(IPollRepository pollRepository)
        {
            _pollRepository = pollRepository;
        }
        public async Task<Poll> CreatePoll(Poll poll)
        {
            try
            {
                // we need to check bussiness logic to validate before save
                return await _pollRepository.AddAsync(poll);

            }
            catch (Exception e)
            {
                // todo pending custom exepcion? disscuss with team
                throw new NotImplementedException("Error creating poll: "+e.Message);
            }
        }
        public async Task<Poll?> GetPollById(int pollId)
        {
            return await _pollRepository.GetByIdAsync(pollId);
        }
    }
}
