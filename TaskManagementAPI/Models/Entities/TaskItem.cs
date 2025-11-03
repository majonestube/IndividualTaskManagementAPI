namespace TaskManagementAPI.Models.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Status { get; set; }
    public DateTime DueDate { get; set; }
    public int ProjectId { get; set; }
    public int AssignedUserId { get; set; }
}