using Microsoft.AspNetCore.Identity;

namespace TaskManagementAPI.Models.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    
    public int StatusId { get; set; }
    public Status Status { get; set; }
    
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    
    public string? AssignedUserId { get; set; }
    public IdentityUser? AssignedUser { get; set; }
    
    public ICollection<Comment> Comments { get; set; }
}