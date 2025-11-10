namespace TaskManagementAPI.Models.DTO;

public class ProjectDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; }
    public string Username { get; set; }
    public int TaskCount { get; set; }
}