using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.PollInstances.Commands.UpdatePollInstance;
public class UpdatePollInstanceByIdCommandHandler: 
    IRequestHandler<UpdatePollInstanceByIdCommand, CreateCommandResponse<PollInstance>>
{
    private IPollInstanceRepository _pollInstanceRepository;
    private ILogger<UpdatePollInstanceByIdCommandHandler> _logger;

    public UpdatePollInstanceByIdCommandHandler(IPollInstanceRepository PollInstanceRepository,
        ILogger<UpdatePollInstanceByIdCommandHandler> Logger) 
    {
        _pollInstanceRepository = PollInstanceRepository;
        _logger = Logger;
    }

    public async Task<CreateCommandResponse<PollInstance>> Handle
        (UpdatePollInstanceByIdCommand Request, CancellationToken CancellationToken)
    {
        var query = await _pollInstanceRepository.GetByIdAsync(Request.PollInstanceDTO.Id);
        if (query == null)
            return new CreateCommandResponse<PollInstance>(new PollInstance(),"Poll Instance Not Found",false,
                Models.Enums.CommandEnums.CommandResultStatus.NotFound);
        var updatedPoll = await _pollInstanceRepository.UpdateAsync(Request.PollInstanceDTO.ToDomain());
        return new CreateCommandResponse<PollInstance>(updatedPoll, "Updated Poll Instance", true);
    }
}
