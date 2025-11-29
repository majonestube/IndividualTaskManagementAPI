using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Frontend.Services;

public class JwtHelper
{
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