using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Students.Queries.GetAll;
using Eras.Application.Models.Enums;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.PollVersions.Queries.GetByPollAndVersion;
public class GetPollVersionByPollAndVersionQueryHandler: 
    IRequestHandler<GetPollVersionByPollAndVersionQuery, GetQueryResponse<PollVersion>>
{
    private ILogger<GetPollVersionByPollAndVersionQueryHandler> _logger;
    private IPollVersionRepository _pollVersionRepository;
    public GetPollVersionByPollAndVersionQueryHandler(IPollVersionRepository PollVersionRepository,
            ILogger<GetPollVersionByPollAndVersionQueryHandler> Logger
        )
    {
        _pollVersionRepository = PollVersionRepository;
        _logger = Logger;
    }
    public async Task<GetQueryResponse<PollVersion>> Handle
        (GetPollVersionByPollAndVersionQuery Request, CancellationToken CancellationToken)
    {
           PollVersion? pollVersion = await _pollVersionRepository.GetByPollAndVersionAsync(Request.VersionName,Request.PollId);
        if ( pollVersion == null ) 
            return new GetQueryResponse<PollVersion>(new PollVersion(),"Poll Not Fond",true,QueryEnums.QueryResultStatus.NotFound);
        return new GetQueryResponse<PollVersion>(pollVersion,"Poll found",true);
    }
}
