﻿using Eras.Application.Models.Consolidator;
using Eras.Application.Models.Response.Common;

using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class PollAvgQuery : IRequest<GetQueryResponse<AvgReportResponseVm>>
{
    public required Guid PollUuid { get; set; }
    public required int CohortId { get; set; }
}
