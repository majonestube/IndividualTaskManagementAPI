using System.Text.Json.Serialization;

namespace MyShared.Models;

public class NotificationCreateDto
{
    public int ProjectId { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TaskItemId { get; set; }
    public string Message { get; set; } = string.Empty;
}