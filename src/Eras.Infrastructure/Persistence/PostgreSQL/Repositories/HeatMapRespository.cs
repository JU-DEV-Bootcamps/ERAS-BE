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
                WHERE pi.uuid = {0}
                ORDER BY c.""Id"", v.""Id"", a.answer_text;";

        private string _getHeatMapDataByVariables = @"
            SELECT
            s.uuid AS student_uuid,
            s.name AS student_name,
            c.name AS component_name,
            v.name AS variable_name,
            a.risk_level AS answer_risk_level,
            a.answer_text AS answer_name

            FROM students s
                JOIN poll_instances pi ON pi.""StudentId"" = s.""Id""
                JOIN student_details sd ON s.""Id"" = sd.""StudentId""
                JOIN answers a ON pi.""Id"" = a.poll_instance_id
                JOIN poll_variable pv ON a.poll_variable_id = pv.""Id""
                JOIN variables v ON pv.variable_id = v.""Id""
                JOIN components c ON v.component_id = c.""Id""

            WHERE c.name = {0} AND pi.uuid = {1}";

        public HeatMapRespository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<
            IEnumerable<GetHeatMapByComponentsQueryResponse>
        > GetHeatMapDataByComponentsAsync(string pollUUID)
        {
            var restult = await _context
                .Database.SqlQueryRaw<GetHeatMapByComponentsQueryResponse>(
                    _getHeatMapDataByComponentsQuery,
                    pollUUID
                )
                .ToListAsync();
            return restult;
        }

        public async Task<IEnumerable<GetHeatMapDetailByVariablesQueryResponse>> GetHeatMapDataByVariables(
            string componentName,
            string pollInstanceUuid)
        {
            var result = await _context
                .Database.SqlQueryRaw<GetHeatMapDetailByVariablesQueryResponse>(
                _getHeatMapDataByVariables,
                componentName,
                pollInstanceUuid
            ).ToListAsync();
            return result;
        }

    }
}
