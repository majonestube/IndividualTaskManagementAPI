using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public class CommentService : ICommentService
{
    private readonly TaskManagementDbContext _db;

    public CommentService(TaskManagementDbContext db)
    {
        _db = db;
    }

    public async Task<List<CommentDto>> GetByTask(int taskItemId)
    {
        // Henter kommentarer for en oppgave og mapper til DTO
        var comments = await _db.Comments
            .AsNoTracking()
            .Include(c => c.TaskItem)
            .Include(c => c.User)
            .Where(c => c.TaskItemId == taskItemId)
            .Select(c => new CommentDto
            {
                Text = c.Text,
                CreatedDate = c.CreatedDate,
                TaskItemName = c.TaskItem.Title,
                Username = c.User.Username
            })
            .ToListAsync();

        return comments;
    }

    public async Task<Comment?> GetById(int id)
    {
        // Henter kommentar etter id
        return await _db.Comments
            .AsNoTracking()
            .Include(c => c.TaskItem)
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task Create(Comment comment)
    {
        // Oppretter ny kommentar etter validering
        var taskExists = await _db.Tasks.AnyAsync(t => t.Id == comment.TaskItemId);
        if (!taskExists)
        {
            throw new Exception("Ugyldig oppgave-id.");
        }

        var userExists = await _db.Users.AnyAsync(u => u.Id == comment.UserId);
        if (!userExists)
        {
            throw new Exception("Ugyldig bruker-id.");
        }

        await _db.Comments.AddAsync(comment);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> Update(Comment comment)
    {
        // Oppdaterer eksisterende kommentar
        var existing = await _db.Comments.FirstOrDefaultAsync(c => c.Id == comment.Id);
        if (existing == null)
        {
            return false;
        }

        existing.Text = comment.Text;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(int id)
    {
        // Sletter kommentar hvis den finnes
        var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
        {
            return false;
        }

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
        return true;
    }
}
