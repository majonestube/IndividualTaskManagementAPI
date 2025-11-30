using Microsoft.EntityFrameworkCore;
using MyShared.Models;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services.CommentServices;

public class CommentService(TaskManagementDbContext db) : ICommentService
{
    private readonly TaskManagementDbContext _db = db;
    public async Task<List<CommentDto>> GetByTask(int taskItemId, string userId)
    {
        if (!await CanAccessProject(taskItemId, userId))
        {
            throw new UnauthorizedAccessException("Bruker har ikke tilgang til prosjektet");
        }
        
        // Henter kommentarer for en oppgave og mapper til DTO
        var comments = await _db.Comments
            .AsNoTracking()
            .Include(c => c.TaskItem)
            .Include(c => c.User)
            .Where(c => c.TaskItemId == taskItemId)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Text = c.Text,
                CreatedDate = c.CreatedDate,
                TaskItemId = c.TaskItemId,
                TaskItemName = c.TaskItem.Title,
                Username = c.User.UserName
            })
            .ToListAsync();

        return comments;
    }

    public async Task<CommentDto?> GetById(int id)
    {
        // Henter kommentar etter id
        var comment = await _db.Comments
            .AsNoTracking()
            .Include(c => c.TaskItem)
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        return comment == null ? null : CommentToDto(comment);
    }

    public async Task Create(string userId, int taskItemId, CommentCreateDto comment)
    {
        // Oppretter ny kommentar etter validering
        if (!await CanAccessProject(taskItemId, userId))
        {
            throw new UnauthorizedAccessException("Bruker har ikke tilgang til prosjektet");
        }
        
        var taskExists = await _db.Tasks.AnyAsync(t => t.Id == taskItemId);
        if (!taskExists)
        {
            throw new BadHttpRequestException("Ugyldig oppgave-id.");
        }

        var userExists = await _db.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new UnauthorizedAccessException("Ugyldig bruker-id.");
        }

        var newComment = new Comment
        {
            Text = comment.Text,
            CreatedDate = DateTime.Now,
            TaskItemId = taskItemId,
            UserId = userId
        };

        await _db.Comments.AddAsync(newComment);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> Update(CommentDto comment, string userId)
    {
        // Oppdaterer eksisterende kommentar
        if (!await IsCommentOwner(comment.Id, userId))
        {
            throw new UnauthorizedAccessException("Bruker har ikke tilgang til denne kommentaren");
        }
        
        var existing = await _db.Comments.FirstOrDefaultAsync(c => c.Id == comment.Id);
        if (existing == null)
        {
            return false;
        }

        existing.Text = comment.Text;
        existing.CreatedDate = comment.CreatedDate;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(int id, string userId)
    {
        // Sletter kommentar hvis den finnes
        if (!await IsCommentOwner(id, userId))
        {
            throw new UnauthorizedAccessException("Bruker har ikke tilgang til denne kommentaren");
        }
        
        var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
        {
            return false;
        }

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
        return true;
    }
    
    // Sjekker om bruker eier kommentaren
    public async Task<bool> IsCommentOwner(int commentId, string userId)
    {
        var comment = await _db.Comments
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == commentId);

        return comment != null && comment.UserId == userId;
    }
    
    // Sjekker om bruker har tilgang til prosjektet
    private async Task<bool> CanAccessProject(int taskId, string userId)
    {
        var projectId = await _db.Tasks
            .Where(t => t.Id == taskId)
            .Select(t => t.ProjectId)
            .FirstOrDefaultAsync();

        if (projectId == 0) return false;

        return await _db.ProjectVisibility
            .AsNoTracking()
            .AnyAsync(pv => pv.ProjectId == projectId && pv.UserId == userId);
    }

    private static CommentDto CommentToDto(Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Text = comment.Text,
            CreatedDate = comment.CreatedDate,
            TaskItemId = comment.TaskItemId,
            TaskItemName = comment.TaskItem?.Title ?? "Unknown title",
            Username = comment.User?.UserName ??  "Unknown username"
        };
    }
}
