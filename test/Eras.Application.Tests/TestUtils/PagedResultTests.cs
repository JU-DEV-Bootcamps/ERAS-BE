using Eras.Application.Utils;

namespace Eras.Application.Tests.TestUtils;

public class PagedResultTests
{
    [Fact]
    public void Constructor_SetsCountAndItems()
    {
        // Arrange
        var items = new List<string> { "Item 1", "Item 2" };

        // Act
        var result = new PagedResult<string>(2, items);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Same(items, result.Items);
    }

    [Fact]
    public void Empty_ReturnsZeroCountAndEmptyItems()
    {
        // Act
        var result = PagedResult<string>.Empty();

        // Assert
        Assert.Equal(0, result.Count);
        Assert.Empty(result.Items);
    }
}