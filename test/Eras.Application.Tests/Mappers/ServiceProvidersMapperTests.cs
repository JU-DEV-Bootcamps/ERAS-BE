using Eras.Application.DTOs;
using Eras.Application.Mappers;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Mappers;

public class ServiceProvidersMapperTests
{
    [Fact]
    public void ToDomain_MapsAllProperties()
    {
        // Arrange
        var dto = new ServiceProvidersDTO
        {
            ServiceProviderName = "Provider Name",
            ServiceProviderLogo = "provider-logo.png",
            Audit = new AuditInfo()
        };

        // Act
        var result = dto.ToDomain();

        // Assert
        Assert.Equal(dto.ServiceProviderName, result.ServiceProviderName);
        Assert.Equal(dto.ServiceProviderLogo, result.ServiceProviderLogo);
        Assert.Equal(dto.Audit, result.Audit);
    }

    [Fact]
    public void ToDto_MapsAllProperties()
    {
        // Arrange
        var domain = new ServiceProviders
        {
            ServiceProviderName = "Provider Name",
            ServiceProviderLogo = "provider-logo.png",
            Audit = new AuditInfo()
        };

        // Act
        var result = domain.ToDto();

        // Assert
        Assert.Equal(domain.ServiceProviderName, result.ServiceProviderName);
        Assert.Equal(domain.ServiceProviderLogo, result.ServiceProviderLogo);
        Assert.Equal(domain.Audit, result.Audit);
    }
}
