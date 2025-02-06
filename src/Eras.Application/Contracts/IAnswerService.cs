using Eras.Domain.Entities;

namespace Eras.Application.Contracts
{
    public interface IAnswerService
    {
        Task<Answer> CreateAnswer(Answer answer, Student student);
    }
}
