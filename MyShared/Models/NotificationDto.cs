namespace MyShared.Models;

public class NotificationDto
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; }
    public int? TaskId { get; set; }
    public string? TaskName { get; set; }
    public string Message { get; set; }
    public DateTime Created { get; set; }
    public bool IsRead { get; set; }
}