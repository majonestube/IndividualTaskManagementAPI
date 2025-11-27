namespace MyShared.Models;

public class ProjectDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; }
    public string Username { get; set; }
    public int TaskCount { get; set; }
    public int UnreadNotificationsCount { get; set; }
}