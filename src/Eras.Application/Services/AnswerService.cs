using Eras.Domain.Entities;
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
        public Answer CreateAnswer(Answer answer, Student student)
        {

            // Create relation between answer and student

            Console.WriteLine("------ Creando answer ------");
            Console.WriteLine(student.Email); // Davidcst2991 @gmail.com
            Console.WriteLine(answer.Id); // 1
            Console.WriteLine(answer.AnswerText);// Davidcst2991 @gmail.com
            Console.WriteLine(answer.Position); // 1
            Console.WriteLine(answer.Question); // Escribe email
            Console.WriteLine(answer.Score); // 0

            /* 
            Aqui deberia unirme con manuel..
            Llamar a interfaz de persistencia, 

            */
            return answer;
        }
    }
}
