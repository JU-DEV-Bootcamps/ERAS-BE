using Eras.Application.DTOs;

using MediatR;

namespace Eras.Application.Features.FeatureFlags;

public sealed record GetAllFeatureFlagsQuery() : IRequest<IReadOnlyCollection<FeatureFlagDTO>>;
public sealed record GetFeatureFlagByNameQuery(string Name) : IRequest<FeatureFlagDTO?>;