using TaskManagementAPI.Models.DTO;

namespace TaskManagementAPI.Services;

public interface INotificationService
{
    Task<List<NotificationDto>> GetNotificationsForUser(int userId);
    Task<NotificationDto> AddNotification(int projectId, int? taskId, int userId, string message);

    Task<bool> MarkAsRead(int notificationId);

    Task<bool> MarkAsUnread(int notificationId);
}