using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Models;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent
{
    public class GetHigherRiskStudentByPollQuery
        : IRequest<GetQueryResponse<List<(Answer answer, Variable variable, Student student)>>>
    {
        public required string PollInstanceUuid { get; set; }
        public int? Take { get; set; }

        public required string VariableIds { get; set; }
    }

}
