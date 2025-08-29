using Eras.Application.Contracts.Persistence;
using Eras.Domain.Common;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Configurations.Queries.GetUserConfigurations;
public class GetUserConfigurationsQueryHandler : IRequestHandler<GetUserConfigurationsQuery, List<Domain.Entities.Configurations>>
{
    private readonly IConfigurationsRepository _configurationsRepository;
    private readonly ILogger<GetUserConfigurationsQueryHandler> _logger;
    private readonly IApiKeyEncryptor _encryptor;
    public GetUserConfigurationsQueryHandler(IConfigurationsRepository ConfigurationsRepository, ILogger<GetUserConfigurationsQueryHandler> Logger, IApiKeyEncryptor Encryptor)
    {
        _configurationsRepository = ConfigurationsRepository;
        _logger = Logger;
        _encryptor = Encryptor;
    }

    public async Task<List<Domain.Entities.Configurations>> Handle(GetUserConfigurationsQuery Request, CancellationToken CancellationToken) 
    {
        var listOfConfigurations = await _configurationsRepository.GetUserConfigurationsAsync(Request.UserId);
        var response = listOfConfigurations.Select(c => 
        {
            if (!string.IsNullOrWhiteSpace(c.EncryptedKey))
            {
                c.EncryptedKey = _encryptor.Decrypt(c.EncryptedKey);
            }
            return c;
        });
        return response.ToList();
    }
}
