namespace TaskManagementAPI.Models.DTO;

public class NotificationCreateDto
{
    public int ProjectId { get; set; }
    public int? TaskItemId { get; set; }
    public string Message { get; set; }
}