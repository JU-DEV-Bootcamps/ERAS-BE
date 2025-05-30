using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IComponentsAvgRepository
    {
        Task<List<ComponentsAvg>> ComponentsAvgByStudent(int StudentId, int PollId);
    }
}
