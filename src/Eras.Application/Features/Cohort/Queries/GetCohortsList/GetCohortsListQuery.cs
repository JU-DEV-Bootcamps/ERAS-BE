using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Cohort.Queries.GetCohortsList
{
    public class GetCohortsListQuery : IRequest<List<Domain.Entities.Cohort>>
    {
    }
}
