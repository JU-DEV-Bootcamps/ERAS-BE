using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IAnswerRepository : IBaseRepository<Answer>
    {
        Task<List<Answer>> GetByPollInstanceIdAsync(string Uuid);
        Task<List<Answer>> GetByStudentIdAsync(string Uuid);
        Task<List<Answer>> GetByPollInstanceAnswerAndPollVariableAsync(int PollVariableId, int PollInstanceId, string AnswerText);
        Task SaveManyAnswersAsync(List<Answer> Answers);
    }
}
