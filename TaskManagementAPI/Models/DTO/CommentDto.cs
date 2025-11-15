namespace TaskManagementAPI.Models.DTO;

public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTime CreatedDate { get; set; }
    public int TaskItemId { get; set; }
    public string TaskItemName { get; set; }
    public string Username { get; set; }
}