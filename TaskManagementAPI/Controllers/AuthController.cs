using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyShared.Models;
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
                Message = "Bruker eksisterer allerede. Vennligst logg inn i stedet."
            });
        }

        if (await authService.RegisterUser(user))
        {
            return Ok(new
            {
                IsSuccess = true,
                Message = "Bruker registrert!"
            });
        }
        
        return BadRequest(new
        {
            IsSuccess = false,
            Message = "Ugyldig brukernavn eller passord." 
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
                Message = "Bruker eksisterer ikke."
            });
        }
        
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Message = "Ugyldig ModelState..."
            });
        }

        if (!await authService.LoginUser(user))
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Message = "Ugyldig brukernavn eller passord."
            });
        }
        
        var tokenString = await authService.GenerateTokenString(identityUser);

        return Ok(new LoginResponse
        {
            IsSuccess = true,
            Token = tokenString,
            Message = "Bruker logget inn!"
        });
    }
}