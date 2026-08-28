using Eras.Application.DTOs;
using Eras.Application.Mappers;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Mappers;

public class ConfigurationMapperTest
{
    [Fact]
    public void ToDomain_Should_Convert_ConfigurationDTO_To_Configuration()
    {
        var dto = new ConfigurationsDTO
        {
            ConfigurationName = "Configuration",
            UserId = "1",
            BaseURL = "123-no]12/example",
            EncryptedKey = "20"
        };
        var result = dto.ToDomain();
        Assert.NotNull(result);
        Assert.Equal(dto.ConfigurationName, result.ConfigurationName);
        Assert.Equal(dto.UserId, result.UserId);
        Assert.Equal(dto.BaseURL, result.BaseURL);
        Assert.Equal(dto.EncryptedKey, result.EncryptedKey);
    }

    [Fact]
    public void ToDto_Should_Convert_Configuration_To_ConfigurationDto()
    {
        var model = new Configurations
        {
            ConfigurationName = "Configuration",
            UserId = "1",
            BaseURL = "123-no]12/example",
            EncryptedKey = "20"
        };
        var result = model.ToDto();
        Assert.NotNull(result);
        Assert.Equal(model.ConfigurationName, result.ConfigurationName);
        Assert.Equal(model.Audit, result.Audit);
        Assert.Equal(model.BaseURL, result.BaseURL);
        Assert.Equal(model.UserId, result.UserId);
    }
}
