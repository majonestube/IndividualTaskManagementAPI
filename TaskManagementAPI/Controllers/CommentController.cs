using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    // Henter kommentarer for en gitt oppgave
    [HttpGet("task/{taskItemId:int}")]
    public async Task<IActionResult> GetForTask(int taskItemId)
    {
        var result = await _commentService.GetByTask(taskItemId);
        return Ok(result); // 200: OK
    }

    // Henter enkel kommentar etter id
    [HttpGet("{id:int}", Name = "GetCommentById")]
    public async Task<IActionResult> GetById(int id)
    {
        var comment = await _commentService.GetById(id);
        if (comment == null)
        {
            return NotFound(); // 404: Ikke funnet
        }

        return Ok(comment);
    }

    // Oppretter en ny kommentar
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Comment comment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // 400: Ugyldig modell
        }

        try
        {
            await _commentService.Create(comment);
            return CreatedAtRoute("GetCommentById", new { id = comment.Id }, comment); // 201: Opprettet
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); // 400: Valideringsfeil
        }
    }

    // Oppdaterer en kommentar
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Comment comment)
    {
        if (id != comment.Id)
        {
            return BadRequest("Id i ruten samsvarer ikke med id i kroppen."); // 400: Ugyldig forespørsel
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updated = await _commentService.Update(comment);
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
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _commentService.Delete(id);
        if (!deleted)
        {
            return NotFound($"Ingen kommentar med id {id} funnet."); // 404: Ikke funnet
        }

        return NoContent();
    }
}