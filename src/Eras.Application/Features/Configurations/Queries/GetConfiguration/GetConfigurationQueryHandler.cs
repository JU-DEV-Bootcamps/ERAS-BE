using Eras.Application.Contracts.Persistence;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Configurations.Queries.GetConfiguration;
public class GetConfigurationQueryHandler : IRequestHandler<GetConfigurationQuery, Domain.Entities.Configurations>
{
    IConfigurationsRepository _configurationsRepository;
    ILogger<GetConfigurationQueryHandler> _logger;
    public GetConfigurationQueryHandler(IConfigurationsRepository ConfigurationsRepository, ILogger<GetConfigurationQueryHandler> Logger)
    {
        _configurationsRepository = ConfigurationsRepository;
        _logger = Logger;
    }

    public async Task<Domain.Entities.Configurations> Handle(GetConfigurationQuery Request, CancellationToken CancellationToken)
    {
        var configuration = await _configurationsRepository.GetByIdAsync(Request.ConfigurationId);
        if (configuration == null)
        {
            _logger.LogError($"Configuration with ID {Request.ConfigurationId} not found.");
            throw new KeyNotFoundException($"Configuration with ID {Request.ConfigurationId} not found.");
        }
        _logger.LogInformation($"Configuration with ID {Request.ConfigurationId} retrieved successfully.");
        return configuration;
    }
}
