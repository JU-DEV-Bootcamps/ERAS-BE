using Eras.Application.Models;
using MediatR;

namespace Eras.Application.Features.Consolidator.Queries;

public class PollAvgQuery: IRequest<BaseResponse>
{
    public required Guid PollUuid { get; set; }
}
