using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Frontend.Services;

public class JwtAuthenticationStateProvider(JwtHelper jwtHelper) : AuthenticationStateProvider
{
    private readonly JwtHelper jwtHelper = jwtHelper;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string? token = null;
        try
        {
            token = await jwtHelper.GetAuthorizationTokenAsync();
        }
        catch (InvalidOperationException)
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            return new AuthenticationState(anonymous);
        }
        
        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        
        var claims = jwtHelper.GetClaimsFromToken(token).ToList();
         var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
         if (nameClaim == null)
         {
             var usernameClaim = claims.FirstOrDefault(c => c.Type == "username");
             if (usernameClaim != null)
             {
                 claims.Add(new Claim(ClaimTypes.Name, usernameClaim.Value));
             }
         } 
        
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }

    public void NotifyAuthChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    
}