using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    // Henter alle brukere
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await userService.GetUsers();
        return Ok(users); // 200: OK
    }

    // Henter bruker etter id
    [HttpGet("{id}", Name = "GetUserById")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await userService.GetUserById(id);
        if (user == null)
        {
            return NotFound($"Ingen bruker med id {id} funnet."); // 404: Ikke funnet
        }

        return Ok(user);
    }

    // Oppretter ny bruker
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // 400: Ugyldig modell
        }

        try
        {
            var createdUser = await userService.Create(dto);
            return CreatedAtRoute("GetUserById", new { id = createdUser.Id }, createdUser); // 201: Opprettet
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); // 400: Valideringsfeil
        }
    }

    // Oppdaterer bruker
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedUser = await userService.Update(id, dto);
            if (updatedUser == null)
            {
                return NotFound($"Ingen bruker med id {id} funnet."); // 404: Ikke funnet
            }

            return Ok(updatedUser); // 200: OK
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Sletter bruker
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var success = await userService.Delete(id);
        if (!success)
        {
            return NotFound($"Ingen bruker med id {id} funnet."); // 404: Ikke funnet
        }

        return NoContent(); // 204: Ingen innhold
    }
}