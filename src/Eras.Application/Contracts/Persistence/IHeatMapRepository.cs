using Eras.Application.Models.Response.HeatMap;

namespace Eras.Application.Contracts.Persistence
{
    public interface IHeatMapRepository
    {
        Task<IEnumerable<GetHeatMapByComponentsQueryResponse>> GetHeatMapDataByComponentsAsync(string PollUUID);
        Task<IEnumerable<GetHeatMapByComponentsQueryResponse>> GetHeatMapDataByCohortAndDaysAsync(int? CohortId, int? Days);
    }
}
