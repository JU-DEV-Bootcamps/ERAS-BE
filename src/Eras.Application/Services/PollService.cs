using Eras.Domain.Entities;
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
        public Poll CreatePoll(Poll poll)
        {
            Console.WriteLine("------ Creando poll ------");
            Console.WriteLine(poll.Id);
            Console.WriteLine(poll.PollName);
            /* 
            Aqui deberia unirme con manuel..
            Llamar a interfaz de persistencia, 
                buscar por ID DE CL,si existe retornar poll
                sino crear nueva poll
                {
                    ValidateNewPoll(poll); // Hay algo que validar? Nombre?
                    Guardar Poll..
                    Retornar poll guardada
                }
            */
            return poll;
        }

        public void ValidateNewPoll(Poll poll)
        {
            Console.WriteLine("Validando que nombre es correcto");
            throw new NotImplementedException();
        }
    }
}
