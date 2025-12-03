using Microsoft.EntityFrameworkCore;
using MyShared.Models;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services.NotificationServices;

public class NotificationService(TaskManagementDbContext db) : INotificationService
{
    private readonly TaskManagementDbContext _db = db;
    
    // Henter alle varslene til en bruker
    public async Task<List<NotificationDto>> GetNotificationsForUser(string userId)
    {
        var notifications = await _db.Notifications
            .AsNoTracking()
            .Include(n => n.Project)
            .Include(n => n.Task)
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.Created)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                ProjectId = n.ProjectId,
                ProjectName = n.Project.Name,
                TaskItemId = n.TaskItemId,
                TaskName = n.Task != null ? n.Task.Title : "",
                Message = n.Message,
                Created = n.Created,
                IsRead = n.IsRead
            })
            .ToListAsync();

        return notifications;
    }
    
    // Legger til varsler til alle med tilgang til prosjektet
    public async Task<bool> AddNotification(int projectId, int? taskId, string message)
    {
        var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists) return false;

        var userIds = await _db.ProjectVisibility
            .AsNoTracking()
            .Where(pv => pv.ProjectId == projectId)
            .Select(pv => pv.UserId)
            .ToListAsync();
        
        if (taskId.HasValue)
        {
            var taskExists = await _db.Tasks
                .AnyAsync(t => t.Id == taskId.Value && t.ProjectId == projectId);

            if (!taskExists)
                return false; 
        }

        foreach (var newNotification in userIds.Select(userId => new Notification
                 {
                     ProjectId = projectId,
                     TaskItemId = taskId,
                     UserId = userId,
                     Message = message,
                     Created = DateTime.UtcNow,
                     IsRead = false
                 }))
        {
            await _db.Notifications.AddAsync(newNotification);
        }
        
        await _db.SaveChangesAsync();

        return true;
    }

    // Markerer varsel som lest
    public async Task<bool> MarkAsRead(int notificationId)
    {
        var notification = await _db.Notifications.FindAsync(notificationId);
        if (notification == null) return false;

        notification.IsRead = true;
        await _db.SaveChangesAsync();
        return true;
    }

    // Markerer varsel som ulest 
    public async Task<bool> MarkAsUnread(int notificationId)
    {
        var notification = await _db.Notifications.FindAsync(notificationId);
        if (notification == null) return false;

        notification.IsRead = false;
        await _db.SaveChangesAsync();
        return true;
    }
}