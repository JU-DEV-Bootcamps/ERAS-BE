using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Common;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Configurations.Command.CreateConfiguration;
public class CreateConfigurationCommandHandler : IRequestHandler<CreateConfigurationCommand, CreateCommandResponse<Domain.Entities.Configurations>>
{
    private readonly IConfigurationsRepository _configurationsRepository;
    private readonly ILogger<CreateConfigurationCommandHandler> _logger;
    private readonly IApiKeyEncryptor _encryptor;

    public CreateConfigurationCommandHandler(IConfigurationsRepository ConfigurationsRepository, ILogger<CreateConfigurationCommandHandler> Logger, IApiKeyEncryptor Encryptor)
    {
        _configurationsRepository = ConfigurationsRepository;
        _logger = Logger;
        _encryptor = Encryptor;
    }

    public async Task<CreateCommandResponse<Domain.Entities.Configurations>> Handle(CreateConfigurationCommand Request, CancellationToken CancellationToken)
    {
        try
        {
            if (Request.Configurations == null)
            {
                _logger.LogError("Configuration is null");
                return new CreateCommandResponse<Domain.Entities.Configurations>(null, 0, "Error", false);
            }

            Domain.Entities.Configurations? configurationDB = await _configurationsRepository.GetByNameAsync(Request.Configurations.ConfigurationName);
            if (configurationDB != null) return new CreateCommandResponse<Domain.Entities.Configurations>(configurationDB, 0, "Success", true);

            Domain.Entities.Configurations? configuration = Request.Configurations?.ToDomain();

            if (configuration == null)
                return new CreateCommandResponse<Domain.Entities.Configurations>(null, 0, "Error", false);

            if (!string.IsNullOrWhiteSpace(configuration.EncryptedKey))
            {
                configuration.EncryptedKey = _encryptor.Encrypt(configuration.EncryptedKey);
            }

            Domain.Entities.Configurations createdConfiguration = await _configurationsRepository.AddAsync(configuration);

            return new CreateCommandResponse<Domain.Entities.Configurations>(createdConfiguration, 1, "Success", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating the component: ");
            return new CreateCommandResponse<Domain.Entities.Configurations>(null, 0, "Error", false);
        }
    }
}
