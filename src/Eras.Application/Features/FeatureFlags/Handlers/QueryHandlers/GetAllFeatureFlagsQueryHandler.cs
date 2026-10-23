using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Mappers;
using Eras.Domain.Entities.FeatureFlagManagement;

using MediatR;

namespace Eras.Application.Features.FeatureFlags.Handlers.QueryHandlers;
public class GetAllFeatureFlagsQueryHandler
    : IRequestHandler<GetAllFeatureFlagsQuery, IReadOnlyCollection<FeatureFlagDTO>>
{
    private readonly IFeatureFlagRepository _repository; 
    public GetAllFeatureFlagsQueryHandler(IFeatureFlagRepository Repository)
    {
        _repository = Repository;
    }
    public async Task<IReadOnlyCollection<FeatureFlagDTO>> Handle(
        GetAllFeatureFlagsQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<FeatureFlag> entities = await _repository.GetAllAsync();
        return entities.Select(Entity => Entity.ToDTO()).ToList();
    }
}