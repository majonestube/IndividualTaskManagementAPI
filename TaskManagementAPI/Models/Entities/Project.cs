namespace TaskManagementAPI.Models.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public int UserId { get; set; }
    
    public User User { get; set; }
    public ICollection<TaskItem> Tasks { get; set; }
}