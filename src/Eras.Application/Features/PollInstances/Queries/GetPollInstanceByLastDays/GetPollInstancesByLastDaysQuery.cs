﻿using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.PollInstances.Queries.GetPollInstanceByLastDays
{
    public class GetPollInstancesByLastDaysQuery : IRequest<QueryManyResponse<PollInstance>>
    {
        public int LastDays;
    }
}
