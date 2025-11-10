using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public interface ICommentService
{
    // Hent kommentarer for en gitt oppgave
    Task<List<CommentDto>> GetByTask(int taskItemId);
    // Hent kommentar etter id
    Task<Comment?> GetById(int id);
    // Opprett ny kommentar
    Task Create(Comment comment);
    // Oppdater kommentar
    Task<bool> Update(Comment comment);
    // Slett kommentar
    Task<bool> Delete(int id);
}


