using Microsoft.AspNetCore.Identity;

namespace TaskManagementAPI.Models.Entities;

public class ProjectVisibility
{
    public int Id { get; set; }
    
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
}