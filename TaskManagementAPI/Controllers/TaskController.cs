using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;
using TaskManagementAPI.Services;
using TaskManagementAPI.Services.TaskServices;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    // Henter oppgaver for et gitt prosjekt
    [Authorize]
    [HttpGet("project/{projectId:int}")]
    public async Task<IActionResult> GetForProject(int projectId)
    {
        var result = await taskService.GetTasksForProject(projectId);
        return Ok(result); // 200: OK
    }

    // Henter enkelt oppgave etter id
    [Authorize]
    [HttpGet("{id:int}", Name = "GetTaskById")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await taskService.GetById(id);
        if (task == null)
        {
            return NotFound(); // 404: Ikke funnet
        }

        return Ok(task);
    }

    // Oppretter en ny oppgave
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskItemCreateDto task)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // 400: Ugyldig modell
        }

        try
        {
            await taskService.Create(task);
            return Ok(task);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); // 400: Valideringsfeil
        }
    }

    // Oppdaterer en eksisterende oppgave
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskItemCreateDto task)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updated = await taskService.Update(id, task);
            if (!updated)
            {
                return NotFound($"Ingen oppgave med id {id} funnet."); // 404: Ikke funnet
            }

            return NoContent(); // 204: Ingen innhold
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Sletter en oppgave
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await taskService.Delete(id);
        if (!deleted)
        {
            return NotFound($"Ingen oppgave med id {id} funnet."); // 404: Ikke funnet
        }

        return NoContent();
    }

    // Oppdaterer status på oppgaven
    [Authorize]
    [HttpPut("{id:int}/status/{statusId:int}")]
    public async Task<IActionResult> UpdateStatus(int id, int statusId)
    {
        try
        {
            var ok = await taskService.UpdateStatus(id, statusId);
            if (!ok)
            {
                return NotFound($"Ingen oppgave med id {id} funnet."); // 404: Ikke funnet
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // Get possible users for a task
    [Authorize]
    [HttpGet("{taskId:int}/assign/possibleUsers")]
    public async Task<IActionResult> AssignPossibleUsers(int taskId)
    {
        try
        {
            var users = await taskService.GetUsers(taskId);
            return Ok(users);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // Tildeler oppgaven til en bruker
    [Authorize]
    [HttpPut("{id:int}/assign/{userId}")]
    public async Task<IActionResult> AssignUser(int id, string userId)
    {
        try
        {
            var ok = await taskService.AssignUser(id, userId);
            if (!ok)
            {
                return NotFound($"Ingen oppgave med id {id} funnet."); // 404: Ikke funnet
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}