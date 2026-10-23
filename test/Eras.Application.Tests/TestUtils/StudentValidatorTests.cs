using Eras.Application.DTOs;
using Eras.Application.Utils;

namespace Eras.Application.Tests.TestUtils;

public class StudentValidatorTests
{
    [Fact]
    public void IsStudentValid_ReturnsTrue_WhenNameAndEmailAreValid()
    {
        // Arrange
        var student = new StudentImportDto
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            SISId = "852963"
        };

        // Act
        var result = StudentValidator.isStudentValid(student);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsStudentValid_ReturnsFalse_WhenNameIsInvalid()
    {
        // Arrange
        var student = new StudentImportDto
        {
            Name = "John123",
            Email = "john.doe@example.com",
            SISId = "852963"
        };

        // Act
        var result = StudentValidator.isStudentValid(student);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsStudentValid_ReturnsFalse_WhenEmailIsInvalid()
    {
        // Arrange
        var student = new StudentImportDto
        {
            Name = "John Doe",
            Email = "john.doe",
            SISId = "852963"
        };

        // Act
        var result = StudentValidator.isStudentValid(student);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsStudentValid_ReturnsTrue_WhenNameContainsHyphen()
    {
        // Arrange
        var student = new StudentImportDto
        {
            Name = "Mary-Jane Doe",
            Email = "mary.jane@example.com",
            SISId = "789465"
        };

        // Act
        var result = StudentValidator.isStudentValid(student);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsStudentValid_ReturnsTrue_WhenNameContainsApostrophe()
    {
        // Arrange
        var student = new StudentImportDto
        {
            Name = "O'Connor",
            Email = "connor@example.com",
            SISId = "21345"
        };

        // Act
        var result = StudentValidator.isStudentValid(student);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsStudentValid_ReturnsFalse_WhenEmailHasInvalidFormat()
    {
        // Arrange
        var student = new StudentImportDto
        {
            Name = "John Doe",
            Email = "john@",
            SISId = "45678913"
        };

        // Act
        var result = StudentValidator.isStudentValid(student);

        // Assert
        Assert.False(result);
    }
}
