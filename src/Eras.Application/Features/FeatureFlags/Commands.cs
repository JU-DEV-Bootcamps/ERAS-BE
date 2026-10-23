using Eras.Application.DTOs;

using MediatR;

namespace Eras.Application.Features.FeatureFlags;

public sealed record CreateFeatureFlagCommand(FeatureFlagDTO FeatureFlag) : IRequest<FeatureFlagDTO>;
public sealed record UpdateFeatureFlagCommand(FeatureFlagDTO FeatureFlag) : IRequest<FeatureFlagDTO>;
public sealed record DeleteFeatureFlagCommand(int Id) : IRequest;