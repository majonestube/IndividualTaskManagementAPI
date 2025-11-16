using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Services;
using TaskManagementAPI.Services.NotificationServices;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    [Authorize]
    [HttpGet("notifications/user/{userId}")]
    public async Task<IActionResult> GetNotificationsForUser(string userId)
    {
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
    
    [HttpPost]
    public async Task<IActionResult> AddNotification([FromBody] NotificationCreateDto dto)
    {
        var success = await notificationService.AddNotification(dto.ProjectId, dto.TaskItemId, dto.Message);
        return success ? NoContent() : BadRequest();
    }
    
    [Authorize]
    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var success = await notificationService.MarkAsRead(id);
        return success ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpPut("{id:int}/unread")]
    public async Task<IActionResult> MarkAsUnread(int id)
    {
        var success = await notificationService.MarkAsUnread(id);
        return success ? NoContent() : NotFound();
    }
}