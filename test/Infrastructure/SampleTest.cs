namespace ERAS.Infrastructure.Tests
{

    public class SampleTest
    {
        [Fact]
        public async Task AuthenticateAsync_WithValidToken_ShouldReturnAuthenticatedUserId()
        {
            // Arrange
            var service = new ExternalService();
            var token = "valid_token";

            // Act
            var result = await service.AuthenticateAsync(token);

            // Assert
            Assert.Equal("Authenticated User ID: 12345", result);
        }
    }
    public class ExternalService
    {
        public Task<string> AuthenticateAsync(string token)
        {
            if (token == "valid_token")
            {
                return Task.FromResult("Authenticated User ID: 12345");
            }

            return Task.FromResult("Invalid Token");
        }
    }
}
