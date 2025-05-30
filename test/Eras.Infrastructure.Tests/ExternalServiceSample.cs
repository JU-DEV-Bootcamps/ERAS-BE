namespace Eras.Infrastructure.Tests
{
    public class ExternalServiceSample
    {
        public Task<string> AuthenticateAsync(string Token)
        {
            if (Token == "valid_token")
            {
                return Task.FromResult("Authenticated User ID: 12345");
            }

            return Task.FromResult("Invalid Token");
        }
    }
}
