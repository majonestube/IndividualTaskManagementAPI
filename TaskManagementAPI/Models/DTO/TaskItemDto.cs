namespace TaskManagementAPI.Models.DTO;

public class TaskItemDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public DateTime DueDate { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; }
    public string AssignedUserId { get; set; }
    public string AssignedUserName { get; set; }
}