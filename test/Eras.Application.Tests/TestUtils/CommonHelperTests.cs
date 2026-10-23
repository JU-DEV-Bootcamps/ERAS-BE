using Eras.Application.Utils;

namespace Eras.Application.Tests.TestUtils;

public class CommonHelperTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(500000000)]
    [InlineData(2147483647)]
    [InlineData(-2147483647)]
    public void ValidateZeroNumber_ShouldReturnFalse(int value)
    {
        var result = CommonHelper.ValidateZeroNumber(value);

        Assert.False(result);
    }

    [Theory]
    [InlineData(0)]
    public void ValidateZeroNumber_ShouldReturnTrue(int value)
    {
        var result = CommonHelper.ValidateZeroNumber(value);

        Assert.True(result);
    }

    [Theory]
    [InlineData(1.0)]
    [InlineData(-1.0)]
    [InlineData(-7922816251426433759354395033.0)]
    [InlineData(7922816251426433759354395033.9)]
    public void ValidateZeroNumber_ShouldReturnFalseWithDecimal(decimal value)
    {
        var result = CommonHelper.ValidateZeroNumber(value);

        Assert.False(result);
    }
}
