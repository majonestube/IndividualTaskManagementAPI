namespace MyShared.Models;

public class ProjectCreateDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string? UserId { get; set; }
}