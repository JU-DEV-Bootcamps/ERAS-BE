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
            Console.WriteLine("------ Creando answer ------");
            Console.WriteLine(student.Email); // Davidcst2991 @gmail.com
            Console.WriteLine(answer.Id); // 1
            Console.WriteLine(answer.AnswerText);// Davidcst2991 @gmail.com
            Console.WriteLine(answer.Position); // 1
            Console.WriteLine(answer.Question); // Escribe email
            Console.WriteLine(answer.Score); // 0
            Console.WriteLine(answer.ModifiedDate); //1/24/2025 4:47:11 PM
            Console.WriteLine("------ Creando answer ------");

            try
            {
               // await _answerRepository.Add(answer);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
            return answer;
        }
    }
}
