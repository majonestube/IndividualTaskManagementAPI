namespace TaskManagementAPI.Models.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    
    public int StatusId { get; set; }
    public Status Status { get; set; }
    
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    
    public int AssignedUserId { get; set; }
    public User AssignedUser { get; set; }
    
    public ICollection<Comment> Comments { get; set; }
}