using System;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;

public class GetHigherRiskStudentQuery: IRequest<BaseResponse>
{
    public required string cohortId { get; set; }
    public required int takeNumber { get; set; }
}
