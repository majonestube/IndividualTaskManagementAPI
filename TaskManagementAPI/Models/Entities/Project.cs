using Microsoft.AspNetCore.Identity;

namespace TaskManagementAPI.Models.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public string UserId { get; set; }
    
    public IdentityUser User { get; set; }
    public ICollection<TaskItem> Tasks { get; set; }
    public ICollection<ProjectVisibility> ProjectVisibility { get; set; }
    public ICollection<Notification> Notifications { get; set; }
}