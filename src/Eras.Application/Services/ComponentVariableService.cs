using Eras.Application.Dtos;
using Eras.Application.Services;
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
    public class ComponentVariableService : IComponentVariableService
    {

        private readonly IComponentVariableRepository<ComponentVariable> _componentVariableRepository;
        public ComponentVariableService(IComponentVariableRepository<ComponentVariable> componentVariable)
        {
            _componentVariableRepository = componentVariable;
        }
        public async Task<ComponentVariable> CreateVariable(ComponentVariable componentVariable)
        {

            // Create relation between variable and poll

            Console.WriteLine("------ Creando Variable ------");
            Console.WriteLine("Poll id: "+componentVariable.PollId);
            Console.WriteLine("Variable id: " + componentVariable.Id);
            Console.WriteLine("Variable Name: " + componentVariable.Name);
            Console.WriteLine("Variable Position: " + componentVariable.Position);
            try
            {
                await _componentVariableRepository.Add(componentVariable);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
            return componentVariable;

        }
    }
}