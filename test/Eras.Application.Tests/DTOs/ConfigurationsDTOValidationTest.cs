using System.ComponentModel.DataAnnotations;
using Eras.Application.DTOs;
using Eras.Application.Tests.TestUtils;

namespace Eras.Application.Tests.DTOs;
public class ConfigurationsDTOValidationTests
{
    private static ConfigurationsDTO CreateValidDTO() => new ConfigurationsDTO
    {
        Id = 1,
        UserId = "e5c970ab-28d0-4ad0-b6b3-fcbf2a9bd85f",
        ConfigurationName = "Test Configuration",
        BaseURL = "https://testingurl.com/api/test",
        EncryptedKey = "3ncryPT3DT35T",
        ServiceProviderId = 2
    };

    [Fact]
    public void ValidDTO_PassesValidation()
    {
        ConfigurationsDTO dto = CreateValidDTO();

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(-1)]
    public void Id_Negative_FailsRange(int InvalidId)
    {
        ConfigurationsDTO dto = CreateValidDTO();
        dto.Id = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
        
        Assert.Single(results);
        Assert.Equal("Id must be greater or equal to 0.", results.First().ErrorMessage);
        Assert.Contains(nameof(ConfigurationsDTO.Id), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void UserId_Required_FailsWhenMissing(string InvalidUserId)
    {
        ConfigurationsDTO dto = CreateValidDTO();
        dto.UserId = InvalidUserId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("UserId is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(ConfigurationsDTO.UserId), results.First().MemberNames);
    }

    [Fact]
    public void UserId_Fails_WhenLengthTooLong()
    {
        ConfigurationsDTO dto = CreateValidDTO();
        dto.UserId = new string('a', 101);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("UserId must be between 3 and 100 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(ConfigurationsDTO.UserId), results.First().MemberNames);
    }

    [Fact]
    public void UserId_Fails_WhenLengthTooShort()
    {
        ConfigurationsDTO dto = CreateValidDTO();
        dto.UserId = new string('a', 2);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("UserId must be between 3 and 100 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(ConfigurationsDTO.UserId), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void ConfigurationName_Required_FailsWhenMissing(string InvalidConfigurationName)
    {
        ConfigurationsDTO dto = CreateValidDTO();
        dto.ConfigurationName = InvalidConfigurationName;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Configuration name is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(ConfigurationsDTO.ConfigurationName), results.First().MemberNames);
    }

    [Fact]
    public void ConfigurationName_Fails_WhenLengthTooShort()
    {
        ConfigurationsDTO dto = CreateValidDTO();
        dto.ConfigurationName = new string('a', 2);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Configuration Name must have more than 3 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(ConfigurationsDTO.ConfigurationName), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void BaseURL_Required_FailsWhenMissing(string InvalidBaseURL)
    {
        ConfigurationsDTO dto = CreateValidDTO();
        dto.BaseURL = InvalidBaseURL;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Base URL is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(ConfigurationsDTO.BaseURL), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(URLFormatTestData))]
    public void BaseURL_URL_FailsWhenInvalid(string InvalidBaseURL)
    {
        ConfigurationsDTO dto = CreateValidDTO();
        dto.BaseURL = InvalidBaseURL;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Base URL must be a valid URL.", results.First().ErrorMessage);
        Assert.Contains(nameof(ConfigurationsDTO.BaseURL), results.First().MemberNames);
    }

    [Fact]
    public void BaseURL_Fails_WhenLengthTooLong()
    {
        ConfigurationsDTO dto = CreateValidDTO();
        dto.BaseURL = new string("http://example.com/search?") + new string('a', 501);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Base URL must be less than 501 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(ConfigurationsDTO.BaseURL), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void EncryptedKey_Required_FailsWhenMissing(string InvalidEncryptedKey)
    {
        ConfigurationsDTO dto = CreateValidDTO();
        dto.EncryptedKey = InvalidEncryptedKey;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Encrypted key is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(ConfigurationsDTO.EncryptedKey), results.First().MemberNames);
    }

    [Fact]
    public void EncryptedKey_Fails_WhenLengthTooShort()
    {
        ConfigurationsDTO dto = CreateValidDTO();
        dto.EncryptedKey = new string('a', 9);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Encrypted Key must be longer than 10 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(ConfigurationsDTO.EncryptedKey), results.First().MemberNames);
    }

    [Theory]
    [InlineData(-1)]
    public void ServiceProviderId_Negative_FailsRange(int InvalidServiceProviderId)
    {
        ConfigurationsDTO dto = CreateValidDTO();
        dto.ServiceProviderId = InvalidServiceProviderId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
        
        Assert.Single(results);
        Assert.Equal("Service Provider Id must be greater or equal to 0.", results.First().ErrorMessage);
        Assert.Contains(nameof(ConfigurationsDTO.ServiceProviderId), results.First().MemberNames);
    }
}