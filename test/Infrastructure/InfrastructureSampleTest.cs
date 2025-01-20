namespace ERAS.Infrastructure.Tests
{

    public class InfrastructureSampleTest
    {
        [Fact]
        public async Task AuthenticateAsync_WithValidToken_ShouldReturnAuthenticatedUserId()
        {
            // Arrange
            var service = new ExternalServiceSample();
            var token = "valid_token";

            // Act
            var result = await service.AuthenticateAsync(token);

            // Assert
            Assert.Equal("Authenticated User ID: 12345", result);
        }
    }
}
