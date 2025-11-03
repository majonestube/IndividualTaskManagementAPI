using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Models.DTO;

public class TaskItemDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Status Status { get; set; }
    public DateTime DueDate { get; set; }
    public string ProjectName { get; set; }
    public string AssignedUserName { get; set; }
}