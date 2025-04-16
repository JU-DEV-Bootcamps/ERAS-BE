using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface IPollInstanceRepository : IBaseRepository<PollInstance>
{
    Task<PollInstance?> GetByUuidAsync(string Uuid);
    Task<PollInstance?> GetByUuidAndStudentIdAsync(string Uuid, int StudentId);

    Task<IEnumerable<PollInstance>> GetByLastDays(int Days);

    Task<IEnumerable<PollInstance>> GetByCohortIdAndLastDays(int? CohortId, int? Days);

    Task<IEnumerable<Answer>> GetAnswersByPollInstanceUuidAsync(string PollUuid);
}
