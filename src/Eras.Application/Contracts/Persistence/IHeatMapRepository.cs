using Eras.Application.Models.HeatMap;

namespace Eras.Application.Contracts.Persistence
{
    public interface IHeatMapRepository
    {
        Task<IEnumerable<GetHeatMapByComponentsQueryResponse>> GetHeatMapDataByComponentsAsync(string pollUUID);
        Task<IEnumerable<GetHeatMapDetailByVariablesQueryResponse>> GetHeatMapDataByVariables(
            string componentName,
            string pollInstanceUuid);
    }
}
