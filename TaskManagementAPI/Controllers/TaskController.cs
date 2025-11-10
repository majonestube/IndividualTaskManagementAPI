using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    // Henter oppgaver for et gitt prosjekt
    [HttpGet("project/{projectId:int}")]
    public async Task<IActionResult> GetForProject(int projectId)
    {
        var result = await taskService.GetTasksForProject(projectId);
        return Ok(result); // 200: OK
    }

    // Henter enkelt oppgave etter id
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
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskItem task)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // 400: Ugyldig modell
        }

        try
        {
            await taskService.Create(task);
            return CreatedAtRoute("GetTaskById", new { id = task.Id }, task); // 201: Opprettet
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); // 400: Valideringsfeil
        }
    }

    // Oppdaterer en eksisterende oppgave
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskItem task)
    {
        if (id != task.Id)
        {
            return BadRequest("Id i ruten samsvarer ikke med id i kroppen."); // 400: Ugyldig forespørsel
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updated = await taskService.Update(task);
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

    // Tildeler oppgaven til en bruker
    [HttpPut("{id:int}/assign/{userId:int}")]
    public async Task<IActionResult> AssignUser(int id, int userId)
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