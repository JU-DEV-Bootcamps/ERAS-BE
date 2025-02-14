using System;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;

public class GetHigherRiskStudentQuery: IRequest<BaseResponse>
{
    public required Student Student { get; set; }
}
