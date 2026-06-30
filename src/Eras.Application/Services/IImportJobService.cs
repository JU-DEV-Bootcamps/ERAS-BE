using Eras.Application.Dtos;
using Eras.Application.DTOs;

namespace Eras.Application.Services
{
    public interface IImportJobService
    {
        Task<int> StartExtractionAsync(string EvaluationSetName, int ConfigurationId, string? StartDate, string? EndDate, int EvaluationId);
        Task<bool> ConfirmImportAsync(int ImportJobId, List<int> ItemIds);
        Task<int> QueueImportAsync(List<PollDTO> Polls, int EvaluationId);
        Task<ImportJobStatusDTO?> GetStatusAsync(int ImportJobId);
        Task<List<ImportJobItemDTO>> GetItemsAsync(int ImportJobId);
        Task<bool> RetryItemsAsync(int ImportJobId, List<int> ItemIds);
    }
}
