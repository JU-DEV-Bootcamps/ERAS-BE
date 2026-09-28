using Eras.Application.Utils;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Tests.Services;

public class AttachmentEntityTypeRegistryTest
{
    [Theory]
    [InlineData(InterventionConstants.AttachmentEntityType)]
    [InlineData(AttachmentDraftSession.AttachmentEntityType)]
    public void IsRegistered_Should_ReturnTrue_ForRegisteredEntityTypes(string entityType)
    {
        Assert.True(AttachmentEntityTypeRegistry.IsRegistered(entityType));
    }

    [Theory]
    [InlineData("Unknown")]
    [InlineData("temp")] // lowercase — the registry is case-sensitive (Ordinal comparison)
    [InlineData("")]
    public void IsRegistered_Should_ReturnFalse_ForUnregisteredEntityTypes(string entityType)
    {
        Assert.False(AttachmentEntityTypeRegistry.IsRegistered(entityType));
    }
}
