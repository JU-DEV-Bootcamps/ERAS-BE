using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.PollVersions.Commands.CreatePollVersion;
public class CreatePollVersionCommandHandler: 
    IRequestHandler<CreatePollVersionCommand, CreateCommandResponse<PollVersion>>
{
    private IPollVersionRepository _pollVersionRepository;
    private ILogger<CreatePollVersionCommandHandler> _logger;

    public CreatePollVersionCommandHandler(IPollVersionRepository PollVersionRepository, 
        ILogger<CreatePollVersionCommandHandler> Logger)
    {
        _pollVersionRepository = PollVersionRepository;
        _logger = Logger;
    }
    public async Task<CreateCommandResponse<PollVersion>> Handle(CreatePollVersionCommand Request, 
        CancellationToken CancellationToken) 
    {
        PollVersion pollVersion = Request.PollVersionDTO.ToDomain();
        pollVersion.PollId = Request.PollId;
        PollVersion createdPollVersion = await _pollVersionRepository.AddAsync(pollVersion);
        return new CreateCommandResponse<PollVersion>(createdPollVersion,"Created Successfully",true);
    }
}
