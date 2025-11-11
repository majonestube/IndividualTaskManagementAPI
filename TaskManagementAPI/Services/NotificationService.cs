using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public class NotificationService(TaskManagementDbContext db) : INotificationService
{
    public async Task<List<NotificationDto>> GetNotificationsForUser(int userId)
    {
        var notifications = await db.Notifications
            .AsNoTracking()
            .Include(n => n.Project)
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.Created)
            .Select(n => new NotificationDto
            {
                ProjectName = n.Project.Name,
                TaskName = n.Task != null ? n.Task.Title : "",
                Message = n.Message,
                Created = n.Created,
                IsRead = n.IsRead
            })
            .ToListAsync();

        return notifications;
    }
    
    
    public async Task<NotificationDto> AddNotification(int projectId, int? taskId, int userId, string message)
    {
        var projectExists = await db.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists) throw new Exception("Invalid project ID");

        var userExists = await db.Users.AnyAsync(u => u.Id == userId);
        if (!userExists) throw new Exception("Invalid user ID");

        var newNotification = new Notification
        {
            ProjectId = projectId,
            TaskItemId = taskId,
            UserId = userId,
            Message = message,
            Created = DateTime.UtcNow,
            IsRead = false
        };

        await db.Notifications.AddAsync(newNotification);
        await db.SaveChangesAsync();

        return new NotificationDto
        {
            ProjectName = (await db.Projects.FindAsync(projectId))?.Name ?? "Unknown",
            TaskName = taskId.HasValue ? (await db.Tasks.FindAsync(taskId.Value))?.Title ?? "" : "",
            Message = message,
            Created = newNotification.Created,
            IsRead = false
        };
    }

    public async Task<bool> MarkAsRead(int notificationId)
    {
        var notification = await db.Notifications.FindAsync(notificationId);
        if (notification == null) return false;

        notification.IsRead = true;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAsUnread(int notificationId)
    {
        var notification = await db.Notifications.FindAsync(notificationId);
        if (notification == null) return false;

        notification.IsRead = false;
        await db.SaveChangesAsync();
        return true;
    }
}