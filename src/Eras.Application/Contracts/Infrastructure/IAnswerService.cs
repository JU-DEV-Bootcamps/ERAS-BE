using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Infrastructure
{
    public interface IAnswerService
    {
        Task<Answer> CreateAnswer(Answer Answer, Student student);
    }
}
