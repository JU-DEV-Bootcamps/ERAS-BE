using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.PollInstances.Queries.GetByUuidAndStudentId;
public class GetPollInstanceByUuidAndStudentIdQueryHandler :
    IRequestHandler<GetPollInstanceByUuidAndStudentIdQuery, GetQueryResponse<PollInstance>>
{
    private IPollInstanceRepository _pollInstanceRepository;
    private ILogger<GetPollInstanceByUuidAndStudentIdQueryHandler> _logger;

    public GetPollInstanceByUuidAndStudentIdQueryHandler(IPollInstanceRepository PollInstanceRepository,
        ILogger<GetPollInstanceByUuidAndStudentIdQueryHandler> Logger) 
    { 
        _pollInstanceRepository = PollInstanceRepository;
        _logger = Logger;
    }
    public async Task<GetQueryResponse<PollInstance>> Handle
        (GetPollInstanceByUuidAndStudentIdQuery Request, CancellationToken CancellationToken)
    {
        var query = await _pollInstanceRepository.GetByUuidAndStudentIdAsync(Request.PollUuid, Request.StudentId);
        if (query == null) 
            return new GetQueryResponse<PollInstance>(new PollInstance(), "Not Found", true,
                Models.Enums.QueryEnums.QueryResultStatus.NotFound);
        return new GetQueryResponse<PollInstance>(query, "Poll Found", true);

    }
    
}
