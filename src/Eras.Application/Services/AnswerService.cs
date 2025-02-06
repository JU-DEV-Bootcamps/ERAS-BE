using Eras.Application.Contracts;
using Eras.Domain.Entities;
using Eras.Domain.Repositories;

namespace Eras.Application.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository _answerRepository;
        public AnswerService(IAnswerRepository answerRepository)
        {
            _answerRepository = answerRepository;
        }
        public async Task<Answer> CreateAnswer(Answer answer, Student student)
        {
            try
            {
                // TODO
                // WE NEED TO CHECK DB TO SAVE A ANSWERS,
                // WE COULD CREATE ANOTHER TABLE TO HAS A LIST UNION MANY STUDENTS TO A SPECIFIC QUESTION? TO AVOID DUPLICATED ANWSERS..

                // we need to check bussiness logic to validate before save

                return await _answerRepository.AddAsync(answer);
            }
            catch (Exception e)
            {
                // todo pending custom exepcion? disscuss with team
                throw new NotImplementedException("Error creating answer: " + e.Message);
            }
        }
    }
}
