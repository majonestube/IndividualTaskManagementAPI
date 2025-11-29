using MyShared.Models;

namespace TaskManagementAPI.Services.NotificationServices;

public interface INotificationService
{
    Task<List<NotificationDto>> GetNotificationsForUser(string userId);
    Task<bool> AddNotification(int projectId, int? taskId, string message);

    Task<bool> MarkAsRead(int notificationId);

    Task<bool> MarkAsUnread(int notificationId);
}