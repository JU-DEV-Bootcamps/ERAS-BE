using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IAnswerRepository : IBaseRepository<Answer>
    {
        Task SaveManyAnswersAsync(List<Answer> answers);
    }
}