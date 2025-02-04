using Eras.Application.Contracts;
using Eras.Domain.Entities;
using Eras.Domain.Repositories;

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
            try
            {
                // we need to check bussiness logic to validate before save
                return await _componentVariableRepository.Add(componentVariable);
            }
            catch (Exception e)
            {
                // todo pending custom exepcion? disscuss with team
                throw new NotImplementedException("Error creating variable: " + e.Message);
            }
        }

        public async Task<List<ComponentVariable>> GetAllVariables(int pollId)
        {
            return await _componentVariableRepository.GetAll(pollId);
        }
    }
}