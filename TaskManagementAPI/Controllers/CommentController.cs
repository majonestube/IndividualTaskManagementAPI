using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShared.Models;
using TaskManagementAPI.Services.CommentServices;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController(ICommentService commentService) : ControllerBase
{
    // Henter kommentarer for en gitt oppgave
    [Authorize]
    [HttpGet("task/{taskItemId:int}")]
    public async Task<IActionResult> GetByTask(int taskItemId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await commentService.GetByTask(taskItemId, userId);
            return Ok(result); 
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }

    // Henter enkel kommentar etter id
    [Authorize]
    [HttpGet("{id:int}", Name = "GetCommentById")]
    public async Task<IActionResult> GetById(int id)
    {
        var comment = await commentService.GetById(id);
        if (comment == null)
        {
            return NotFound(); 
        }

        return Ok(comment);
    }

    // Oppretter en ny kommentar
    [Authorize]
    [HttpPost("{taskId:int}")]
    public async Task<IActionResult> Create(int taskId, [FromBody] CommentCreateDto comment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); 
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }
        
        try
        {
            await commentService.Create(userId, taskId, comment);
            return Ok(comment);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); 
        }
    }

    // Oppdaterer en kommentar
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CommentDto comment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var updated = await commentService.Update(comment, userId);
            if (!updated)
            {
                return NotFound($"Ingen kommentar med id {id} funnet."); 
            }

            return NoContent(); 
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
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var deleted = await commentService.Delete(id, userId);
            if (!deleted)
            {
                return NotFound($"Ingen kommentar med id {id} funnet."); 
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
}