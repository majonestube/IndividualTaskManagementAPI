using TaskManagementAPI.Models.DTO;

namespace TaskManagementAPI.Services.NotificationServices;

public interface INotificationService
{
    Task<List<NotificationDto>> GetNotificationsForUser(string userId);
    Task<NotificationDto> AddNotification(int projectId, int? taskId, string userId, string message);

    Task<bool> MarkAsRead(int notificationId);

    Task<bool> MarkAsUnread(int notificationId);
}