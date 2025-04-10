using System.Net;

namespace Eras.Infrastructure.Tests;
public class MockHttpMessageHandler(Dictionary<string, HttpResponseMessage> Responses) : DelegatingHandler
{
    private readonly Dictionary<string, HttpResponseMessage> _responses = Responses;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage Req, CancellationToken CancellationToken)
    {
        // Obtiene la URL de la solicitud
        var requestUri = Req.RequestUri?.ToString() ?? string.Empty;

        // Verifica si hay una respuesta configurada para esta URL
        if (_responses.TryGetValue(requestUri, out var response))
        {
            return Task.FromResult(response);
        }

        // Si no hay respuesta configurada, devuelve un 404 Not Found
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
    }
}
