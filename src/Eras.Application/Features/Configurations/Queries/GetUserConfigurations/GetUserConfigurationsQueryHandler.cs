using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Configurations.Queries.GetAllConfigurations;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Configurations.Queries.GetUserConfigurations;
public class GetUserConfigurationsQueryHandler : IRequestHandler<GetUserConfigurationsQuery, List<Domain.Entities.Configurations>>
{
    private readonly IConfigurationsRepository _configurationsRepository;
    private readonly ILogger<GetUserConfigurationsQueryHandler> _logger;
    public GetUserConfigurationsQueryHandler(IConfigurationsRepository ConfigurationsRepository, ILogger<GetUserConfigurationsQueryHandler> Logger)
    {
        _configurationsRepository = ConfigurationsRepository;
        _logger = Logger;
    }

    public async Task<List<Domain.Entities.Configurations>> Handle(GetUserConfigurationsQuery Request, CancellationToken CancellationToken) 
    {
        var listOfConfigurations = await _configurationsRepository.GetUserConfigurationsAsync(Request.UserId);
        return listOfConfigurations.ToList();
    }
}
