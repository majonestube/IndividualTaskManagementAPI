using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;

namespace Frontend.Services;

public class JwtHelper(IHttpClientFactory httpFactory, ProtectedSessionStorage sessionStorage)
{
    private readonly IHttpClientFactory _httpFactory = httpFactory;
    private readonly ProtectedSessionStorage _sessionStorage = sessionStorage;

    public async Task<HttpClient> GetAuthenticatedClientAsync(string clientName = "TaskManagementAPI")
    {
        var tokenResult = await _sessionStorage.GetAsync<string>("authToken");
        var token = tokenResult.Value;

        var http = _httpFactory.CreateClient(clientName);
        
        if (!string.IsNullOrEmpty(token))
        {
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return http;
    }
    public static string? GetUserId(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        return jwt.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.NameIdentifier ||
            c.Type == "sub" ||
            c.Type == "id"
        )?.Value;
    }
}