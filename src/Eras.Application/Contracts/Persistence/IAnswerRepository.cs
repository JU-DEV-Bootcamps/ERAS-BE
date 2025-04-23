using Eras.Application.Models.Response.HeatMap;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface IAnswerRepository : IBaseRepository<Answer>
{
    Task<List<Answer>> GetByPollInstanceIdAsync(string Uuid);
    Task<List<Answer>> GetByStudentIdAsync(string Uuid);
    Task SaveManyAnswersAsync(List<Answer> Answers);
    Task<List<AnswersReportQueryResponse>> GetAnswersByPollInstanceUuidCohortAsync(string PollUuid, string CohortId);
    Task<List<AnswersReportQueryResponse>> GetAnswersByPollVariablesAsync(List<int> PollVariableIds);
}
