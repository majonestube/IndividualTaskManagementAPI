namespace TaskManagementAPI.Models.DTO;

public class CommentCreateDto
{
    public string Text { get; set; }
    public int TaskItemId { get; set; }
}