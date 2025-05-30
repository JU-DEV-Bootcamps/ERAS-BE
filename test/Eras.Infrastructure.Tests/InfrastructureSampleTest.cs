﻿namespace Eras.Infrastructure.Tests
{

    public class InfrastructureSampleTest
    {
        [Fact]
        public async Task AuthenticateWithValidTokenShouldReturnAuthenticatedUserIdAsync()
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
