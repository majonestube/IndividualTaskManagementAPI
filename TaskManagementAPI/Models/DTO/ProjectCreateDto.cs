namespace TaskManagementAPI.Models.DTO;

public class ProjectCreateDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int UserId { get; set; }
}