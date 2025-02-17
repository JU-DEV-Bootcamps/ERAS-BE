using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.HeatMap;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class HeatMapRespository : IHeatMapRepository
    {
        protected readonly AppDbContext _context;

        private string _getHeatMapDataByComponentsQuery = @"
            SELECT
                c.""Id"" AS component_id, c.name AS component_name,
                v.""Id"" AS variable_id,
                v.name AS variable_name, a.answer_text, a.risk_level AS answer_risk_level

            FROM variables v
                JOIN components c ON v.component_id = c.""Id""
                JOIN poll_variable pv ON v.""Id"" = pv.variable_id
                JOIN answers a ON pv.""Id"" = a.poll_variable_id
                JOIN poll_instances pi ON a.poll_instance_id = pi.""Id""

            WHERE pi.uuid = {0};";

        public HeatMapRespository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<GetHeatMapByComponentsQueryResponse>> GetHeatMapDataByComponentsAsync(string pollUUID)
        {
            var restult = await _context.Database.SqlQueryRaw<GetHeatMapByComponentsQueryResponse>(_getHeatMapDataByComponentsQuery, pollUUID).ToListAsync();
            Console.WriteLine(restult);

            return restult;
        }
    }
}
