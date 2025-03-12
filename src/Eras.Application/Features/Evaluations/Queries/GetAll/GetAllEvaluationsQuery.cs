using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Evaluations.Queries.GetAll
{
    public class GetAllEvaluationsQuery : IRequest<List<Evaluation>>
    {
    }
}
