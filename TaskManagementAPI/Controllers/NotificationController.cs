using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    [HttpGet("notifications/user/{userId:int}")]
    public async Task<IActionResult> GetNotificationsForUser(int userId)
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
    
    [HttpPost("{userId:int}")]
    public async Task<IActionResult> AddNotification(int userId, [FromBody] NotificationCreateDto dto)
    {
        try
        {
            var notification = await notificationService.AddNotification(dto.ProjectId, dto.TaskItemId, userId, dto.Message);
            return Ok(notification);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var success = await notificationService.MarkAsRead(id);
        return success ? NoContent() : NotFound();
    }

    [HttpPut("{id:int}/unread")]
    public async Task<IActionResult> MarkAsUnread(int id)
    {
        var success = await notificationService.MarkAsUnread(id);
        return success ? NoContent() : NotFound();
    }
}