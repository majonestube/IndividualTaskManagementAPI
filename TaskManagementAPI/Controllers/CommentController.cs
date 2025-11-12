using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController(ICommentService commentService) : ControllerBase
{
    // Henter kommentarer for en gitt oppgave
    [Authorize]
    [HttpGet("task/{taskItemId:int}")]
    public async Task<IActionResult> GetForTask(int taskItemId)
    {
        var result = await commentService.GetByTask(taskItemId);
        return Ok(result); // 200: OK
    }

    // Henter enkel kommentar etter id
    [Authorize]
    [HttpGet("{id:int}", Name = "GetCommentById")]
    public async Task<IActionResult> GetById(int id)
    {
        var comment = await commentService.GetById(id);
        if (comment == null)
        {
            return NotFound(); // 404: Ikke funnet
        }

        return Ok(comment);
    }

    // Oppretter en ny kommentar
    [Authorize]
    [HttpPost("{taskId:int}/user/{userId}")]
    public async Task<IActionResult> Create(int taskId, string userId, [FromBody] CommentCreateDto comment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // 400: Ugyldig modell
        }

        try
        {
            //TODO string userId = GetCurrentUserId();
            await commentService.Create(userId, taskId, comment);
            return Ok(comment);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); // 400: Valideringsfeil
        }
    }

    // Oppdaterer en kommentar
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CommentCreateDto comment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updated = await commentService.Update(id, comment);
            if (!updated)
            {
                return NotFound($"Ingen kommentar med id {id} funnet."); // 404: Ikke funnet
            }

            return NoContent(); // 204: Ingen innhold
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Sletter en kommentar
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await commentService.Delete(id);
        if (!deleted)
        {
            return NotFound($"Ingen kommentar med id {id} funnet."); // 404: Ikke funnet
        }

        return NoContent();
    }
}