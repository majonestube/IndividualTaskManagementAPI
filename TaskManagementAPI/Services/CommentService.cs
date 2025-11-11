using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public class CommentService(TaskManagementDbContext db) : ICommentService
{
    public async Task<List<CommentDto>> GetByTask(int taskItemId)
    {
        // Henter kommentarer for en oppgave og mapper til DTO
        var comments = await db.Comments
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

    public async Task<CommentDto?> GetById(int id)
    {
        // Henter kommentar etter id
        var comment = await db.Comments
            .AsNoTracking()
            .Include(c => c.TaskItem)
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        return comment == null ? null : CommentToDto(comment);
    }

    public async Task Create(int userId, int taskItemId, CommentCreateDto comment)
    {
        // Oppretter ny kommentar etter validering
        var taskExists = await db.Tasks.AnyAsync(t => t.Id == taskItemId);
        if (!taskExists)
        {
            throw new Exception("Ugyldig oppgave-id.");
        }

        var userExists = await db.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new Exception("Ugyldig bruker-id.");
        }

        var newComment = new Comment
        {
            Text = comment.Text,
            CreatedDate = DateTime.Now,
            TaskItemId = taskItemId,
            UserId = userId
        };

        await db.Comments.AddAsync(newComment);
        await db.SaveChangesAsync();
    }

    public async Task<bool> Update(int commentId, CommentCreateDto comment)
    {
        // Oppdaterer eksisterende kommentar
        var existing = await db.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
        if (existing == null)
        {
            return false;
        }

        existing.Text = comment.Text;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(int id)
    {
        // Sletter kommentar hvis den finnes
        var comment = await db.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
        {
            return false;
        }

        db.Comments.Remove(comment);
        await db.SaveChangesAsync();
        return true;
    }

    private static CommentDto CommentToDto(Comment comment)
    {
        return new CommentDto
        {
            Text = comment.Text,
            CreatedDate = comment.CreatedDate,
            TaskItemName = comment.TaskItem?.Title ?? "Unknown title",
            Username = comment.User?.Username ??  "Unknown username"
        };
    }
}
