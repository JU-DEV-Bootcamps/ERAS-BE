using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Infrastructure;
using Eras.Domain.Entities;

namespace Eras.Application.Services
{
    public class VariableService : IVariableService
    {

        private readonly IVariableRepository _VariableRepository;
        public VariableService(IVariableRepository Variable)
        {
            _VariableRepository = Variable;
        }
        public async Task<Variable> CreateVariable(Variable Variable)
        {
            try
            {
                // we need to check bussiness logic to validate before save
                return await _VariableRepository.AddAsync(Variable);
            }
            catch (Exception e)
            {
                // todo pending custom exepcion? disscuss with team
                throw new NotImplementedException("Error creating variable: " + e.Message);
            }
        }

        public async Task<List<Variable>> GetAllVariables(int PollId)
        {
            return await _VariableRepository.GetAllAsync(PollId);
        }
    }
}