using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Frontend.Services;

public class ApiAuthenticator(JwtStore jwtStore): DelegatingHandler
{
    private readonly JwtStore jwtStore = jwtStore;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await jwtStore.GetToken();
        if (!string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("Adding bearer token");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}