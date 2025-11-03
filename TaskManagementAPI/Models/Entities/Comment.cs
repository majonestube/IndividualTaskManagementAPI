namespace TaskManagementAPI.Models.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTime CreatedDate { get; set; }
    public int TaskItemId { get; set; }
    public int UserId { get; set; }
}