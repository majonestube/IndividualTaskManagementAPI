namespace MyShared.Models;

public class TaskItemCreateDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int StatusId { get; set; } = 1;
    public DateTime DueDate { get; set; }
    public int ProjectId { get; set; }
    public string? AssignedUserId { get; set; }
}