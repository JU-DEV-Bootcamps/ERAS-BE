using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Configurations.Queries.GetAllConfigurations;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.ServiceProviders.Queries;
public class GetAllServiceProvidersQueryHandler : IRequestHandler<GetAllServiceProvidersQuery, List<Domain.Entities.ServiceProviders>>
{
    private readonly IServiceProvidersRepository _serviceProvidersRepository;
    private readonly ILogger<GetAllServiceProvidersQueryHandler> _logger;
    public GetAllServiceProvidersQueryHandler(IServiceProvidersRepository ServiceProvidersRepository, ILogger<GetAllServiceProvidersQueryHandler> Logger)
    {
        _serviceProvidersRepository = ServiceProvidersRepository;
        _logger = Logger;
    }

    public async Task<List<Domain.Entities.ServiceProviders>> Handle(GetAllServiceProvidersQuery request, CancellationToken cancellationToken)
    {
        var listOfServiceProviders = await _serviceProvidersRepository.GetAllAsync();
        return listOfServiceProviders.ToList();
    }
}
