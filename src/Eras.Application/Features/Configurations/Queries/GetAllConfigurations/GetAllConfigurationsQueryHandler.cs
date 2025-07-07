using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Components.Queries;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Configurations.Queries.GetAllConfigurations;
public class GetAllConfigurationsQueryHandler : IRequestHandler<GetAllConfigurationsQuery, List<Domain.Entities.Configurations>>
{
    private readonly IConfigurationsRepository _configurationsRepository;
    private readonly ILogger<GetAllConfigurationsQueryHandler> _logger;

    public GetAllConfigurationsQueryHandler(IConfigurationsRepository ConfigurationsRepository, ILogger<GetAllConfigurationsQueryHandler> Logger)
    {
        _configurationsRepository = ConfigurationsRepository;
        _logger = Logger;
    }

    public async Task<List<Domain.Entities.Configurations>> Handle(GetAllConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var listOfConfigurations = await _configurationsRepository.GetAllAsync();
        return listOfConfigurations.ToList();
    }
}
