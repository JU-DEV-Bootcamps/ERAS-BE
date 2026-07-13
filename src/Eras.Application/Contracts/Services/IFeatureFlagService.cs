namespace Eras.Application.Contracts.Services;

public interface IFeatureFlagService
{
    Task<bool> UseEnhancedEvaluationImport();
    Task<bool> IsV2Enabled();
}