using Microsoft.AspNetCore.Identity;
using MyShared.Models;
using TaskManagementAPI.Models.DTO;

namespace TaskManagementAPI.Services.AuthServices;

public interface IAuthService
{
    Task<bool> RegisterUser(LoginDto user);
    Task<bool> LoginUser(LoginDto user);
    Task<string> GenerateTokenString(IdentityUser user);
}