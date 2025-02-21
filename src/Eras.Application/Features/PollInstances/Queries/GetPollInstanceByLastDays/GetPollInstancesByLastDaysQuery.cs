using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.DTOs;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.PollInstances.Queries.GetPollInstanceByLastDays
{
    public class GetPollInstancesByLastDaysQuery : IRequest<QueryManyResponse<PollInstance>>
    {
        public int LastDays;
    }
}
