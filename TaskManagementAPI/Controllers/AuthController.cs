using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Services.AuthServices;

namespace TaskManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService, UserManager<IdentityUser> userManager)
    : ControllerBase
{
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser(LoginDto user)
    {
        var identityUser = await userManager.FindByNameAsync(user.Username);
        if (identityUser != null)
        {
            return Conflict(new
            {
                IsSuccess = false,
                Message = "User already exists. Please log in instead."
            });
        }

        if (await authService.RegisterUser(user))
        {
            return Ok(new
            {
                IsSuccess = true,
                Message = "User registered successfully!"
            });
        }
        
        return BadRequest(new
        {
            IsSuccess = false,
            Message = "Invalid username or password." 
        });
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginUser(LoginDto user)
    {
        var identityUser = await userManager.FindByNameAsync(user.Username);
        if (identityUser == null)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Message = "User does not exist."
            });
        }
        
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Message = "Invalid ModelState..."
            });
        }

        if (!await authService.LoginUser(user))
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Message = "Invalid username or password."
            });
        }

        return Ok(new
        {
            IsSuccess = true,
            Token = authService.GenerateTokenString(identityUser),
            Message = "User successfully logged in!"
        });
    }
}