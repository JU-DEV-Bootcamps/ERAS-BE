using Eras.Application.Dtos;
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
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository<Answer> _answerRepository;
        public AnswerService(IAnswerRepository<Answer> answerRepository)
        {
            _answerRepository = answerRepository;
        }
        public async Task<Answer> CreateAnswer(Answer answer, Student student)
        {
            try
            {
                // WE NEED TO CHECK DB TO SAVE A ANSWERS,
                // WE COULD CREATE ANOTHER TABLE TO HAS A LIST UNION MANY STUDENTS TO A SPECIFIC QUESTION?
                // AVOID DUPLICATED ANWSERS..


                return await _answerRepository.Add(answer);
            }
            catch (Exception e)
            {
                throw new NotImplementedException("Error creating answer: " + e.Message);
            }
        }
    }
}
