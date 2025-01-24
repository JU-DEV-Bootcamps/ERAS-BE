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
            Console.WriteLine("------ Creando poll ------");
            Console.WriteLine(poll.Id);
            Console.WriteLine(poll.PollName);
            try
            {
                await _pollRepository.Add(poll);

            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
            return poll;
        }

        public void ValidateNewPoll(Poll poll)
        {
            Console.WriteLine("Validando que nombre es correcto");
            throw new NotImplementedException();
        }
    }
}
