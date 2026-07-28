using System.ComponentModel.DataAnnotations;

using Eras.Application.DTOs;
using Eras.Application.Tests.TestUtils;

namespace Eras.Application.Tests.DTOs;
public class ServiceProvidersDTOValidationTests
{
    private static ServiceProvidersDTO CreateValidDTO() => new ServiceProvidersDTO
    {
        ServiceProviderName = "Test Service Provider",
        ServiceProviderLogo = "http://test-provider.com/logo"
    };

    [Fact]
    public void ValidDTO_PassesValidation()
    {
        ServiceProvidersDTO dto = CreateValidDTO();

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void ServiceProviderName_Required_FailsWhenMissing(string InvalidName)
    {
        ServiceProvidersDTO dto = CreateValidDTO();
        dto.ServiceProviderName = InvalidName;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
    
        Assert.Single(results);
        Assert.Equal("Service provider name is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(ServiceProvidersDTO.ServiceProviderName), results.First().MemberNames);
    }

    [Fact]
    public void ServiceProviderName_Fails_WhenLengthTooLong()
    {
        ServiceProvidersDTO dto = CreateValidDTO();
        dto.ServiceProviderName = new string('a', 256);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Service provider name must be between 3 and 255 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(ServiceProvidersDTO.ServiceProviderName), results.First().MemberNames);
    }

    [Fact]
    public void ServiceProviderName_Fails_WhenLengthTooShort()
    {
        ServiceProvidersDTO dto = CreateValidDTO();
        dto.ServiceProviderName = new string('a', 2);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Service provider name must be between 3 and 255 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(ServiceProvidersDTO.ServiceProviderName), results.First().MemberNames);
    }

    [Theory]
    [InlineData("Provider!")]
    [InlineData("Provider #2")]
    [InlineData("Provider|audio")]
    public void ServiceProviderName_Fails_WhenRegExpDoesNotMatch(string InvalidName)
    {
        ServiceProvidersDTO dto = CreateValidDTO();
        dto.ServiceProviderName = InvalidName;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Service provider name can only contain letters, numbers, spaces, dashes, dots, and the '&' symbol.", results.First().ErrorMessage);
        Assert.Contains(nameof(ServiceProvidersDTO.ServiceProviderName), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void ServiceProviderLogo_Required_FailsWhenMissing(string InvalidId)
    {
        ServiceProvidersDTO dto = CreateValidDTO();
        dto.ServiceProviderLogo = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
    
        Assert.Single(results);
        Assert.Equal("Logo URL is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(ServiceProvidersDTO.ServiceProviderLogo), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(URLFormatTestData))]
    public void BaseURL_URL_FailsWhenInvalid(string InvalidBaseURL)
    {
        ServiceProvidersDTO dto = CreateValidDTO();
        dto.ServiceProviderLogo = InvalidBaseURL;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Logo URL must be a valid URL.", results.First().ErrorMessage);
        Assert.Contains(nameof(ServiceProvidersDTO.ServiceProviderLogo), results.First().MemberNames);
    }

    [Fact]
    public void ServiceProviderLogo_Fails_WhenLengthTooLong()
    {
        ServiceProvidersDTO dto = CreateValidDTO();
        dto.ServiceProviderLogo = new string("http://test.com/search?") + new string('a', 10485760);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Logo URL must not exceed 10485760 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(ServiceProvidersDTO.ServiceProviderLogo), results.First().MemberNames);
    }
}