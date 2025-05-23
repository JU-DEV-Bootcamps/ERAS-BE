using System.Net;

namespace Eras.Infrastructure.Tests
{
    public class MockHttpMessageHandler : DelegatingHandler
    {
        private readonly Dictionary<string, HttpResponseMessage> _responses;
        public MockHttpMessageHandler(Dictionary<string, HttpResponseMessage> Responses)
        {
            _responses = Responses;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage Request, CancellationToken CancellationToken)
        {
            if (Request != null && Request.RequestUri != null)
            {
                // Obtiene la URL de la solicitud
                var requestUri = Request.RequestUri.ToString();

                // Verifica si hay una respuesta configurada para esta URL
                if (_responses.TryGetValue(requestUri, out var response))
                {
                    return Task.FromResult(response);
                }
            }

            // Si no hay respuesta configurada, devuelve un 404 Not Found
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}
