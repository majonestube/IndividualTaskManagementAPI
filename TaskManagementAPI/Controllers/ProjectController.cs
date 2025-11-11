using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController(IProjectService projectService) : ControllerBase
{
    // Get all visible projects for the user
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetVisibleProjects(int userId)
    {
        var result = await projectService.GetAllVisibleProjects(userId);
        return Ok(result);
    }
    
    
    // Henter prosjekter for en gitt bruker
    [HttpGet("user/owner/{userId:int}")]
    public async Task<IActionResult> GetForUser(int userId)
    {
        var result = await projectService.GetProjectsForUser(userId);
        return Ok(result); // 200: OK
    }

    // Henter enkelt prosjekt etter id
    [HttpGet("{id:int}", Name = "GetProjectById")]
    public async Task<IActionResult> GetById(int id)
    {
        var project = await projectService.GetById(id);
        if (project == null)
        {
            return NotFound(); // 404: Ikke funnet
        }

        return Ok(project);
    }

    // Oppretter et nytt prosjekt
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProjectCreateDto project)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // 400: Ugyldig modell
        }

        try
        {
            await projectService.Create(project);
            return Ok(project); // Ok
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); // 400: Valideringsfeil
        }
    }

    // Oppdaterer et eksisterende prosjekt
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProjectCreateDto project)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updated = await projectService.Update(id, project);
            if (!updated)
            {
                return NotFound($"Ingen prosjekt med id {id} funnet."); // 404: Ikke funnet
            }

            return NoContent(); // 204: Ingen innhold
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Sletter et prosjekt
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await projectService.Delete(id);
        if (!deleted)
        {
            return NotFound($"Ingen prosjekt med id {id} funnet."); // 404: Ikke funnet
        }

        return NoContent();
    }
}
