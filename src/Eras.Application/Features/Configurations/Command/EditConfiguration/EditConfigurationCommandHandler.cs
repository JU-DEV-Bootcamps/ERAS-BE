using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Configurations.Command.DeleteConfiguration;
using Eras.Application.Features.Evaluations.Commands;
using Eras.Application.Features.Evaluations.Commands.UpdateEvaluation;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Configurations.Command.EditConfiguration;
public class EditConfigurationCommandHandler : IRequestHandler<EditConfigurationCommand, CreateCommandResponse<Domain.Entities.Configurations>>
{
    private readonly IConfigurationsRepository _configurationsRepository;
    private readonly IServiceProvidersRepository _serviceProvidersRepository;
    private readonly ILogger<EditConfigurationCommandHandler> _logger;
    private readonly IApiKeyEncryptor _encryptor;
    public EditConfigurationCommandHandler(IConfigurationsRepository ConfigurationsRepository, IServiceProvidersRepository ServiceProvidersRepository, ILogger<EditConfigurationCommandHandler> Logger, IApiKeyEncryptor Encryptor)
    {
        _configurationsRepository = ConfigurationsRepository;
        _serviceProvidersRepository = ServiceProvidersRepository;
        _logger = Logger;
        _encryptor = Encryptor;
    }

    public async Task<CreateCommandResponse<Domain.Entities.Configurations>> Handle(EditConfigurationCommand Request, CancellationToken CancellationToken)
    {
        try
        {
            Domain.Entities.Configurations? configurationDB = await _configurationsRepository.GetByIdAsyncNoTracking(Request.ConfigurationDTO.Id);

            if (configurationDB == null)
            {
                _logger.LogWarning("Configuration with ID {Id} not found", Request.ConfigurationDTO.Id);
                return new CreateCommandResponse<Domain.Entities.Configurations>(null, 0, "Configuration not found", false);
            }

            // Fields always updatable
            configurationDB.ConfigurationName = Request.ConfigurationDTO.ConfigurationName;
            configurationDB.ServiceProviderId = Request.ConfigurationDTO.ServiceProviderId;
            configurationDB.BaseURL = Request.ConfigurationDTO.BaseURL;
            configurationDB.EncryptedKey = _encryptor.Encrypt(Request.ConfigurationDTO.EncryptedKey);
            configurationDB.Audit.ModifiedAt = DateTime.UtcNow;
            configurationDB.Audit.ModifiedBy = Request.ConfigurationDTO.UserId;
            
            Domain.Entities.ServiceProviders? serviceProvider = await _serviceProvidersRepository.GetByIdAsync(Request.ConfigurationDTO.ServiceProviderId);
            configurationDB.ServiceProvider = serviceProvider;
            
            await _configurationsRepository.UpdateAsync(configurationDB);

            _logger.LogInformation("Successfully updated Configuration ID {Id}", configurationDB.Id);
            return new CreateCommandResponse<Domain.Entities.Configurations>(configurationDB, 1, "Success", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred updating the evaluation: " + Request.ConfigurationDTO.ConfigurationName);
            return new CreateCommandResponse<Domain.Entities.Configurations>(null, 0, "Error", false);
        }
    }
}
