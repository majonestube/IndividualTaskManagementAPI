using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.DTO;

namespace TaskManagementAPI.Services.AuthServices;

public class AuthService(UserManager<IdentityUser> userManager, IConfiguration config)
    : IAuthService
{
    public async Task<bool> RegisterUser(LoginDto user)
    {
        var identityUser = new IdentityUser
        {
            UserName = user.Username,
            Email = user.Username
        };
        
        var result = await userManager.CreateAsync(identityUser, user.Password);
        return result.Succeeded;
    }
    
    public async Task<bool> LoginUser(LoginDto user)
    {
        var identityUser = await userManager.FindByNameAsync(user.Username);
        if (identityUser is null)
        {
            return false;
        }
        return await userManager.CheckPasswordAsync(identityUser, user.Password);
    }
    
    public async Task<string> GenerateTokenString(IdentityUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };
        
        // get roles
        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var jwtKey = config.GetSection("Jwt:Key").Value;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

        var securityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            issuer: config.GetSection("Jwt:Issuer").Value,
            audience: config.GetSection("Jwt:Audience").Value,
            signingCredentials: signingCredentials
        );
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
        Console.Write("Bearer " + tokenString); // Just to print to terminal during testing/developing
        return tokenString;
    }
}