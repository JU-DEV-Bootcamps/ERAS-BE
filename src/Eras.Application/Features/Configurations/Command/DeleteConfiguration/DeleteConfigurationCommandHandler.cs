using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Evaluations.Commands.DeleteEvaluation;
using Eras.Application.Models.Response;
using Eras.Application.Models.Response.Common;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Configurations.Command.DeleteConfiguration;
public class DeleteConfigurationCommandHandler : IRequestHandler<DeleteConfigurationCommand, BaseResponse>
{
    private readonly IConfigurationsRepository _configurationsRepository;
    private readonly ILogger<DeleteConfigurationCommandHandler> _logger;
    public DeleteConfigurationCommandHandler(IConfigurationsRepository ConfigurationsRepository, ILogger<DeleteConfigurationCommandHandler> Logger)
    {
        _configurationsRepository = ConfigurationsRepository;
        _logger = Logger;
    }

    public async Task<BaseResponse> Handle(DeleteConfigurationCommand Request, CancellationToken CancellationToken)
    {
        try
        {
            Domain.Entities.Configurations evaluation = await _configurationsRepository.GetByIdAsyncNoTracking(Request.ConfigurationId);

            if (evaluation == null)
            {
                _logger.LogWarning("Configuration with ID {Id} not found", Request.ConfigurationId);
                return new BaseResponse("Configuration not found", false);
            }
            await _configurationsRepository.DeleteAsync(evaluation);
            return new BaseResponse("Configuration deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred deleteing the configuration: " + Request.ConfigurationId);
            return new CreateCommandResponse<Domain.Entities.Configurations>(null, 0, "Error", false);
        }
    }
}
