using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IVariableRepository : IBaseRepository<Variable>
    {
        Task<List<Variable>> GetAllAsync(int PollId);
        Task<Variable?> GetByNameAsync(string Name);
        Task<List<Variable>> GetAllByPollUuidAsync(string PollUuid, List<string> Component, bool LastVersion);
        Task<Variable?> GetByNameAndPollIdAsync(string Name, int PollId);
    }
}
