using Eras.Application.Contracts.Services;
using Eras.Application.DTOs;
using Eras.Application.Features.FeatureFlags;
using Eras.Application.Models;
using Eras.Error.Bussiness;

using MediatR;

namespace Eras.Application.Services;

public sealed class FeatureFlagService(IMediator Mediator) : IFeatureFlagService
{
    private readonly IMediator _mediator = Mediator;

    public async Task<bool> UseEnhancedEvaluationImport() {
        var v2Override = await IsFeatureFlagEnabledAsync(FeatureFlags.Version2);
        if (v2Override)
        {
            return v2Override;
        }

        return await IsFeatureFlagEnabledAsync(FeatureFlags.EnhancedEvaluationsImport);
    }

    public async Task<bool> IsV2Enabled() => await IsFeatureFlagEnabledAsync(FeatureFlags.Version2);

    private async Task<bool> IsFeatureFlagEnabledAsync(string Name)
    {
        try
        {
            FeatureFlagDTO? featureFlag = await _mediator.Send(
                CreateQuery(Name)
            );

            return featureFlag?.IsEnabled ?? false;
        }
        catch(NotFoundException)
        {
            return false;
        }
    }

    private static GetFeatureFlagByNameQuery CreateQuery(string Name) => new GetFeatureFlagByNameQuery(Name);
}