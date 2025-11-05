using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    // Get all users
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await userService.GetUsers();
            return Ok(users);
        }
        catch
        {
            return NotFound();
        }
    }
    
    // Get user by id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        try
        {
            var user = await userService.GetUserById(id);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            
            return Ok(user);
        }
        catch
        {
            return NotFound();
        }
    }
    
    // Create new user
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
    {
        await userService.Create(dto);
        return Ok("User created");
    }
    
    // Update user
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto dto)
    {
        // TODO can we check that ids match? 
        await userService.Update(id, dto);
        
        return Ok("User updated");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var success = await userService.Delete(id);
        return success ? NoContent() : BadRequest("User not found");
    }
}