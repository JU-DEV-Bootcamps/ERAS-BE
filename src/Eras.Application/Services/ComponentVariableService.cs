using Eras.Application.Dtos;
using Eras.Domain.Entities;
using Eras.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Services
{
    public class ComponentVariableService : IComponentVariableService
    {
        public ComponentVariable CreateVariable(ComponentVariable componentVariable)
        {

            // Create relation between variable and poll

            Console.WriteLine("------ Creando Variable ------");
            Console.WriteLine("Poll id: "+componentVariable.PollId);
            Console.WriteLine("Variable id: " + componentVariable.Id);
            Console.WriteLine("Variable Name: " + componentVariable.Name);
            Console.WriteLine("Variable Position: " + componentVariable.Position);
            /* 
            Aqui deberia unirme con manuel..
            Llamar a interfaz de persistencia, guardar variable y retornarla
            */

            return componentVariable;

        }
    }
}
