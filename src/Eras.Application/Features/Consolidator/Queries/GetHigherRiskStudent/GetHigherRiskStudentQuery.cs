using System;
using Eras.Application.Utils;
using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;

public class GetHigherRiskStudentQuery: IRequest<BaseResponse>
{
    public required string CohortId { get; set; }
    public required int TakeNumber { get; set; }
}
