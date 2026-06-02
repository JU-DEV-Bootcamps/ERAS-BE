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
    Task<List<StudentsByFiltersResponse>> GetStudentsByFilters(string PollUuid, List<string> ComponentNames, List<int> CohortIds, List<int>? VariableIds, List<int>? RiskLevel);
}
