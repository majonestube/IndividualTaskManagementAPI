using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Services;
using TaskManagementAPI.Services.ProjectServices;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController(IProjectService projectService) : ControllerBase
{
    // Get all projects
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) 
            return Unauthorized();

        try
        {
            var result = await projectService.GetAllProjects();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // Get all visible projects for the user
    [Authorize]
    [HttpGet("/visible")]
    public async Task<IActionResult> GetVisibleProjects()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) 
            return Unauthorized();

        try
        {
            var result = await projectService.GetAllVisibleProjects(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // Henter prosjekter for en gitt bruker
    [Authorize]
    [HttpGet("/owner")]
    public async Task<IActionResult> GetForUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) 
            return Unauthorized();

        try
        {
            var result = await projectService.GetProjectsForUser(userId);
            return Ok(result); // 200: OK
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }

    // Henter enkelt prosjekt etter id
    [Authorize]
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
    [Authorize]
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
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProjectCreateDto project)
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
            var updated = await projectService.Update(id, project, userId);
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
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }
        
        var deleted = await projectService.Delete(id, userId);
        if (!deleted)
        {
            return NotFound($"Ingen prosjekt med id {id} funnet."); // 404: Ikke funnet
        }

        return NoContent();
    }

    [Authorize]
    [HttpPost("{projectId:int}/share")]
    public async Task<IActionResult> ShareProject(int projectId, [FromBody] ProjectShareDto projectShare)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await projectService.ShareProject(projectId, userId!, projectShare.sharedUserId);
            if (!result)
            {
                return NotFound("Prosjektet ikke funnet");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
