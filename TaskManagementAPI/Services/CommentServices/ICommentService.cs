using MyShared.Models;

namespace TaskManagementAPI.Services.CommentServices;

public interface ICommentService
{
    // Hent kommentarer for en gitt oppgave
    Task<List<CommentDto>> GetByTask(int taskItemId, string userId);
    // Hent kommentar etter id
    Task<CommentDto?> GetById(int id);
    // Opprett ny kommentar
    Task Create(string userId, int taskItemId, CommentCreateDto comment);
    // Oppdater kommentar
    Task<bool> Update(CommentDto comment, string userId);
    // Slett kommentar
    Task<bool> Delete(int id, string userId);
}


