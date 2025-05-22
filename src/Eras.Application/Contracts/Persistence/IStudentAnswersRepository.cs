using Eras.Application.Utils;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IStudentAnswersRepository
    {
        Task<List<StudentAnswer>> GetStudentAnswersAsync(int StudentId, int PollId);

        Task<PagedResult<StudentAnswer>> GetStudentAnswersPagedAsync(int StudentId, int PollId, int Page, int PageSize);
    }
}
