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
               return  await _pollRepository.Add(poll);

            }
            catch (Exception e)
            {
                throw new NotImplementedException("Error creating poll: "+e.Message);
            }
        }

        public void ValidateNewPoll(Poll poll)
        {
            throw new NotImplementedException();
        }
    }
}
