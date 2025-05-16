using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Polls.Commands.UpdatePoll;
public class UpdatePollByIdCommandHandler: IRequestHandler<UpdatePollByIdCommand, 
    CreateCommandResponse<Poll>>
{
    private IPollRepository _pollRepository;
    private ILogger<UpdatePollByIdCommandHandler> _logger;

    public UpdatePollByIdCommandHandler(IPollRepository PollRepository, 
        ILogger<UpdatePollByIdCommandHandler> Logger)
    {
        _pollRepository = PollRepository;
        _logger = Logger;
    }
    public async Task<CreateCommandResponse<Poll>> Handle(UpdatePollByIdCommand Request, 
        CancellationToken CancellationToken) 
    {
        var query = await _pollRepository.GetByIdAsync(Request.PollDTO.Id);
        if (query == null)
            return new CreateCommandResponse<Poll>(new Poll(), "Poll Not Found", true, 
                Models.Enums.CommandEnums.CommandResultStatus.NotFound);
        var updatedPoll = await _pollRepository.UpdateAsync(Request.PollDTO.ToDomain());
        return new CreateCommandResponse<Poll>(updatedPoll, "Poll Updated", true);
    }
}
