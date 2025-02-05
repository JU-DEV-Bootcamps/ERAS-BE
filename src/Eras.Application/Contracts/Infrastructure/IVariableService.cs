using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Infrastructure
{
    public interface IVariableService
    {
        Task<Variable> CreateVariable(Variable componentVariable);
        Task<List<Variable>> GetAllVariables(int pollId);
    }
}
