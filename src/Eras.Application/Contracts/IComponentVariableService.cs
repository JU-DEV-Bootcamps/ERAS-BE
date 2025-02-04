using Eras.Domain.Entities;

namespace Eras.Application.Contracts
{
    public interface IComponentVariableService
    {
        Task<ComponentVariable> CreateVariable(ComponentVariable componentVariable);
        Task<List<ComponentVariable>> GetAllVariables(int pollId);
    }
}
