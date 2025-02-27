using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.Tests
{
    public class MockHttpMessageHandler : DelegatingHandler
    {
        private readonly Dictionary<string, HttpResponseMessage> _responses;
        public MockHttpMessageHandler(Dictionary<string, HttpResponseMessage> responses)
        {
            _responses = responses;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Obtiene la URL de la solicitud
            var requestUri = request.RequestUri.ToString();

            // Verifica si hay una respuesta configurada para esta URL
            if (_responses.TryGetValue(requestUri, out var response))
            {
                return Task.FromResult(response);
            }

            // Si no hay respuesta configurada, devuelve un 404 Not Found
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}
