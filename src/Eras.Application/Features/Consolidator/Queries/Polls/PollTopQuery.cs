using Eras.Application.Models;
using MediatR;

namespace Eras.Application.Features.Consolidator.Queries;

public class PollTopQuery: IRequest<BaseResponse>
{
    public required Guid PollUuid { get; set; }
}
