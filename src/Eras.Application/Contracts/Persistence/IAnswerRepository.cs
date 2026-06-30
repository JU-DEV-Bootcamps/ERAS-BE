using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IAnswerRepository : IBaseRepository<Answer>
    {
        Task<List<Answer>> GetByPollInstanceIdAsync(int Id);
        Task<List<Answer>> GetByStudentIdAsync(string Uuid);
        Task<List<Answer>> GetByPollInstanceAnswerAndPollVariableAsync(int PollVariableId, int PollInstanceId, string AnswerText);
        Task<int?> GetAnswerIdByPollInstanceAndVariableAsync(int PollVariableId, int PollInstanceId);
        Task UpdateAnswerTextAsync(int Id, string AnswerText, decimal RiskLevel);
    }
}
