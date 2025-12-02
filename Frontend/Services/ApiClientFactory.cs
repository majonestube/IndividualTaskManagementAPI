using System.Net.Http.Headers;

namespace Frontend.Services;

public class ApiClientFactory(IHttpClientFactory httpClientFactory, JwtStore jwtStore)
{
    private readonly IHttpClientFactory httpClientFactory = httpClientFactory;
    private readonly JwtStore jwtStore = jwtStore;

    public async Task<HttpClient> CreateClient()
    {
        var client = httpClientFactory.CreateClient("TaskManagementAPI");
        var token = await jwtStore.GetToken();
        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }
}