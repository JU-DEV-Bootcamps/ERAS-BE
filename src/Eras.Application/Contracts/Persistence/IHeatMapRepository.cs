using Eras.Application.Models.Response.HeatMap;

namespace Eras.Application.Contracts.Persistence
{
    public interface IHeatMapRepository
    {
        Task<IEnumerable<GetHeatMapByComponentsQueryResponse>> GetHeatMapDataByComponentsAsync(
            string pollUUID
        );
        Task<IEnumerable<GetHeatMapByComponentsQueryResponse>> GetHeatMapDataByCohortAndDaysAsync(
            int? cohortId,
            int? days
        );

        Task<List<HeatMapBaseData>> GetHeatMapByPollUuidAndVariableIds(
            string pollUuid,
            List<int> variableIds
        );
    }
}
