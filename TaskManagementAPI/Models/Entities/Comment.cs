using Microsoft.AspNetCore.Identity;

namespace TaskManagementAPI.Models.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public int TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; }
    
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
}