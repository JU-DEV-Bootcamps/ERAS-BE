using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Evaluations.Queries.GetByDateRange;

public class GetEvaluationsByDateRangeQuery : IRequest<List<Evaluation>>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
