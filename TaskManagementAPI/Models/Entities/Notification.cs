using Microsoft.AspNetCore.Identity;

namespace TaskManagementAPI.Models.Entities;

public class Notification
{
    public int Id { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime Created { get; set; }
    
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    public int? TaskItemId { get; set; }
    public TaskItem? Task { get; set; }
    
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
}