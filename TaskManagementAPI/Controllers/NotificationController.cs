using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShared.Models;
using TaskManagementAPI.Services.NotificationServices;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    // Hent varsler for innlogget bruker
    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> GetNotificationsForUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }
        
        try
        {
            var notifications = await notificationService.GetNotificationsForUser(userId);
            return Ok(notifications);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    // Legger til varsler for alle med tilgang til prosjektet
    [HttpPost]
    public async Task<IActionResult> AddNotification([FromBody] NotificationCreateDto dto)
    {
        var success = await notificationService.AddNotification(dto.ProjectId, dto.TaskItemId, dto.Message);
        return success ? NoContent() : BadRequest();
    }
    
    // Merk varsel som lest
    [Authorize]
    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var success = await notificationService.MarkAsRead(id);
        return success ? NoContent() : NotFound();
    }

    // Merk varsel om ulest
    [Authorize]
    [HttpPut("{id:int}/unread")]
    public async Task<IActionResult> MarkAsUnread(int id)
    {
        var success = await notificationService.MarkAsUnread(id);
        return success ? NoContent() : NotFound();
    }
}