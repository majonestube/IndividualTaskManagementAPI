using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    // Henter prosjekter for en gitt bruker
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetForUser(int userId)
    {
        var result = await _projectService.GetProjectsForUser(userId);
        return Ok(result); // 200: OK
    }

    // Henter enkelt prosjekt etter id
    [HttpGet("{id:int}", Name = "GetProjectById")]
    public async Task<IActionResult> GetById(int id)
    {
        var project = await _projectService.GetById(id);
        if (project == null)
        {
            return NotFound(); // 404: Ikke funnet
        }

        return Ok(project);
    }

    // Oppretter et nytt prosjekt
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Project project)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // 400: Ugyldig modell
        }

        try
        {
            await _projectService.Create(project);
            return CreatedAtRoute("GetProjectById", new { id = project.Id }, project); // 201: Opprettet
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); // 400: Valideringsfeil
        }
    }

    // Oppdaterer et eksisterende prosjekt
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Project project)
    {
        if (id != project.Id)
        {
            return BadRequest("Id i ruten samsvarer ikke med id i kroppen."); // 400: Ugyldig forespørsel
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updated = await _projectService.Update(project);
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
        var deleted = await _projectService.Delete(id);
        if (!deleted)
        {
            return NotFound($"Ingen prosjekt med id {id} funnet."); // 404: Ikke funnet
        }

        return NoContent();
    }
}
