using Eras.Application.Dtos;
using Eras.Application.DTOs;

namespace Eras.Application.Services
{
    public interface IImportJobService
    {
        Task<int> QueueImportAsync(List<PollDTO> Polls, int EvaluationId);
        Task<ImportJobStatusDTO?> GetStatusAsync(int ImportJobId);
        Task<List<ImportJobItemDTO>> GetItemsAsync(int ImportJobId);
        Task<bool> RetryItemsAsync(int ImportJobId, List<int> ItemIds);
    }
}
