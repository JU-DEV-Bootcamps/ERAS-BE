using Eras.Application.Utils;

namespace Eras.Application.Tests.TestUtils;

public class JsonKeyMapperTest
{
    [Fact]
    public void GetJsonKey_ReturnsPropertyName_WhenPropertyIsNotMapped()
    {
        // Arrange
        const string propertyName = "UnmappedProperty";

        // Act
        var result = JsonKeyMapper.GetJsonKey(propertyName);

        // Assert
        Assert.Equal(propertyName, result);
    }
}
