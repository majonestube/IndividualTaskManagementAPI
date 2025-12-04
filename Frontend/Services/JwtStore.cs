using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Frontend.Services;

public class JwtStore(ProtectedSessionStorage sessionStorage)
{
    private const string JwtKey = "authToken";
    private readonly ProtectedSessionStorage sessionStorage = sessionStorage;

    public async Task<string> GetAuthorizationTokenAsync()
    {
        try
        {
            var result = await sessionStorage.GetAsync<string>(JwtKey);
            if (result.Success)
            {
                return result.Value;
            }
            else
            {
                return null;
            }
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public async Task SetToken(string token)
    {
        await sessionStorage.SetAsync("authToken", token);
    }

    internal async Task ClearToken()
    {
        await sessionStorage.DeleteAsync("authToken");  
    }

}