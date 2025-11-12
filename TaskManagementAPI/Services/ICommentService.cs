using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public interface ICommentService
{
    // Hent kommentarer for en gitt oppgave
    Task<List<CommentDto>> GetByTask(int taskItemId);
    // Hent kommentar etter id
    Task<CommentDto?> GetById(int id);
    // Opprett ny kommentar
    Task Create(string userId, int taskId, CommentCreateDto comment);
    // Oppdater kommentar
    Task<bool> Update(int commentId, CommentCreateDto comment);
    // Slett kommentar
    Task<bool> Delete(int id);
}


