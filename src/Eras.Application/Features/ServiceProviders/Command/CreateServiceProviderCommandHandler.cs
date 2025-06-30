using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Configurations.Command.CreateConfiguration;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.ServiceProviders.Command;
public class CreateServiceProviderCommandHandler : IRequestHandler<CreateServiceProviderCommand, CreateCommandResponse<Domain.Entities.ServiceProviders>>
{
    private readonly IServiceProvidersRepository _serviceProvidersRepository;
    private readonly ILogger<CreateServiceProviderCommandHandler> _logger;

    public CreateServiceProviderCommandHandler(IServiceProvidersRepository ServiceProvidersRepository, ILogger<CreateServiceProviderCommandHandler> Logger)
    {
        _serviceProvidersRepository = ServiceProvidersRepository;
        _logger = Logger;
    }

    public async Task<CreateCommandResponse<Domain.Entities.ServiceProviders>> Handle(CreateServiceProviderCommand Request, CancellationToken CancellationToken)
    {
        try
        {
            if (Request.ServiceProviders == null)
            {
                _logger.LogError("Service Provider is null");
                return new CreateCommandResponse<Domain.Entities.ServiceProviders>(null, 0, "Error", false);
            }
            Domain.Entities.ServiceProviders? serviceProviderDB = await _serviceProvidersRepository.GetByNameAsync(Request.ServiceProviders.ServiceProviderName);
            if (serviceProviderDB != null) return new CreateCommandResponse<Domain.Entities.ServiceProviders>(serviceProviderDB, 0, "Success", true);

            Domain.Entities.ServiceProviders? serviceProvider = Request.ServiceProviders?.ToDomain();
            if (serviceProvider == null) return new CreateCommandResponse<Domain.Entities.ServiceProviders>(null, 0, "Error", false);
            Domain.Entities.ServiceProviders createdServiceProvider = await _serviceProvidersRepository.AddAsync(serviceProvider);

            return new CreateCommandResponse<Domain.Entities.ServiceProviders>(createdServiceProvider, 1, "Success", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating the Service Provider: ");
            return new CreateCommandResponse<Domain.Entities.ServiceProviders>(null, 0, "Error", false);
        }
    }
}
