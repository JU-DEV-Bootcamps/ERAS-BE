using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Models.Consolidator;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.EvaluationDetails.Queries.GetEvaluationDetailsByFilters;

public class GetEvaluationDetailsByFiltersQuery : IRequest<List<ErasEvaluationDetailsView>>
{
    public int? PollId { get; set; }
    public List<int>? ComponentIds { get; set; }
    public List<int>? CohortIds { get; set; }
    public List<int>? VariableIds { get; set; }
    
}