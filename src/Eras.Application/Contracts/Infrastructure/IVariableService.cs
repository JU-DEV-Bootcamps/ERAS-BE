using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Infrastructure
{
    public interface IVariableService
    {
        Task<Variable> CreateVariable(Variable ComponentVariable);
        Task<List<Variable>> GetAllVariables(int PollId);
    }
}
