using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface IErasEvaluationDetailsViewRepository : IBaseRepository<ErasEvaluationDetailsView>
{
    Task<List<ErasEvaluationDetailsView>> GetByFiltersAsync(int? PollId, List<int>? ComponentIds, List<int>? CohortIds, List<int>? VariableIds);
    Task<IEnumerable<ErasEvaluationDetailsView>> GetStudentsByFilters(string PollUuid, List<string> ComponentNames, List<int> CohortIds, List<int>? VariableIds, List<decimal>? RiskLevel, int Page, int PageSize, DateTime startDate, DateTime endDate);
    Task<List<StudentsByFiltersResponse>> GetStudentsByEvaluationIdFilters(int EvaluationId, List<string> ComponentNames, List<int> CohortIds, List<int>? VariableIds, List<decimal>? RiskLevel, DateTime startDate, DateTime endDate);
    Task<int> CountStudentsByFilters(string PollUuid, List<string> ComponentNames, List<int> CohortIds, List<int>? VariableIds, List<decimal>? RiskLevel, DateTime startDate, DateTime endDate);
    Task<IEnumerable<GetStudentsRecentAlertsResponse>> GetRecentAlertsStudentAsync(int Page, int PageSize);
    Task<int> CountRecentAlerts();

}
