using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Frontend.Services;

public class JwtAuthenticationStateProvider(JwtStore jwtStore) : AuthenticationStateProvider
{
    private readonly JwtStore jwtStore = jwtStore;
    private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await jwtStore.GetAuthorizationTokenAsync();

            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(anonymous);
            }

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "JwtAuth");
            
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch (Exception)
        {
            return new AuthenticationState(anonymous);
        }
    }

    private IEnumerable<Claim>? ParseClaimsFromJwt(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        var claims = jwtToken.Claims;
        var tokenClaim = new Claim("raw-token", token);

        return [..claims, tokenClaim];
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await jwtStore.SetToken(token);

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "JwtAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await jwtStore.ClearToken();
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
    }
}