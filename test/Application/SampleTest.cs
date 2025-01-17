using Infrastructure.Persistence.PostgreSQL;

namespace ERAS.Application.Tests
{
    public class SampleTest
    {

        [Fact]
        public void Users_To_UserDto_Mapping_ShouldBeCorrect()
        {
            // Arrange
            var user = new Users
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            // Act: Simulate mapping
            var userDto = new UserDto
            {
                Name = user.Name,
                Email = user.Email
            };

            // Assert
            Assert.Equal(user.Name, userDto.Name);
            Assert.Equal(user.Email, userDto.Email);
        }

    }

    internal class UserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
