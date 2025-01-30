using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Eras.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Services
{
    public class PollService : IPollService
    {

        private readonly IPollRepository<Poll> _pollRepository;
        public PollService(IPollRepository<Poll> pollRepository)
        {
            _pollRepository = pollRepository;
        }
        public async Task<Poll> CreatePoll(Poll poll)
        {
            try
            {
                // we need to check bussiness logic to validate before save
                return await _pollRepository.Add(poll);

            }
            catch (Exception e)
            {
                // todo pending custom exepcion? disscuss with team
                throw new NotImplementedException("Error creating poll: "+e.Message);
            }
        }
        public async Task<Poll> GetPollById(int pollId)
        {
            return await _pollRepository.GetPollById(pollId);
        }
    }
}
