using MyShared.Models;

namespace Frontend.Services;

public class NotificationService(JwtHelper jwtHelper)
{
    private readonly JwtHelper _jwtHelper = jwtHelper;

    public async Task MarkNotificationsAsRead(NotificationDto[]? unreadNotifications)
    {
        if (unreadNotifications == null) return;
        
        var client = await _jwtHelper.GetAuthenticatedClientAsync();

        foreach (var notification in unreadNotifications)
        {
            var response = await client.PutAsJsonAsync($"api/Notification/{notification.Id}/read", notification.Id);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error marking notification as read: " + response.StatusCode);
            }
        }
    }

    public async Task MarkProjectNotificationsAsRead(int projectId)
    {
        var client = await _jwtHelper.GetAuthenticatedClientAsync();
        var notifications = await client.GetFromJsonAsync<NotificationDto[]?>($"api/Notification/user");
        
        if (notifications == null) return;
        
        var unreadNotifications = notifications?.Where(n => !n.IsRead && n.ProjectId == projectId).ToArray();
        await MarkNotificationsAsRead(unreadNotifications);
    }
}