namespace MyShared.Models;

public class NotificationDto
{
    public string ProjectName { get; set; }
    public string? TaskName { get; set; }
    public string Message { get; set; }
    public DateTime Created { get; set; }
    public bool IsRead { get; set; }
}