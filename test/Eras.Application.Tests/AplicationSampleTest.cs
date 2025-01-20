using Eras.Infrastructure.Persistence.PostgreSQL;

namespace Eras.Application.Tests
{
    public class AplicationSampleTest
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
            var userDto = new UserDtoSample
            {
                Name = user.Name,
                Email = user.Email
            };

            // Assert
            Assert.Equal(user.Name, userDto.Name);
            Assert.Equal(user.Email, userDto.Email);
        }

    }
}
