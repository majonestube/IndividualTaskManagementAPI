using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Services.UserServices;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    // Henter alle brukere
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await userService.GetUsers();
        return Ok(users);
    }

    // Henter bruker etter id
    [Authorize]
    [HttpGet("{id}", Name = "GetUserById")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await userService.GetUserById(id);
        if (user == null)
        {
            return NotFound($"Ingen bruker med id {id} funnet."); 
        }

        return Ok(user);
    }

    // Oppdaterer bruker
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) 
            return Unauthorized();

        try
        {
            var updatedUser = await userService.Update(id, dto, userId);
            if (updatedUser == null)
            {
                return NotFound($"Ingen bruker med id {id} funnet.");
            }

            return Ok(updatedUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Sletter bruker
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) 
            return Unauthorized();

        try
        {
            var success = await userService.Delete(id, userId);
            if (!success)
            {
                return NotFound($"Ingen bruker med id {id} funnet."); 
            }

            return NoContent(); 
        } catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
        
    }
    
    // Slett bruker som admin
    [Authorize(Roles = "Admin")]
    [HttpDelete("admin/{id}")]
    public async Task<IActionResult> DeleteUserAsAdmin(string id)
    {
        try
        {
            var success = await userService.DeleteAsAdmin(id);
            if (!success)
            {
                return NotFound($"Ingen bruker med id {id} funnet."); 
            }

            return NoContent(); 
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
}