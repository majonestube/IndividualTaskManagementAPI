using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public class TaskService(TaskManagementDbContext db) : ITaskService
{
    public async Task<List<TaskItemDto>> GetTasksForProject(int projectId)
    {
        // Henter alle oppgaver for et gitt prosjekt og mapper til DTO
        var tasks = await db.Tasks
            .AsNoTracking()
            .Include(t => t.Status)
            .Include(t => t.Project)
            .Include(t => t.AssignedUser)
            .Where(t => t.ProjectId == projectId)
            .Select(t => new TaskItemDto
            {
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                DueDate = t.DueDate,
                ProjectName = t.Project.Name,
                AssignedUserName = t.AssignedUser.Username
            })
            .ToListAsync();

        return tasks;
    }

    public async Task<TaskItem?> GetById(int id)
    {
        // Henter oppgave etter id
        return await db.Tasks
            .AsNoTracking()
            .Include(t => t.Status)
            .Include(t => t.Project)
            .Include(t => t.AssignedUser)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task Create(TaskItem task)
    {
        // Oppretter ny oppgave etter validering
        var projectExists = await db.Projects.AnyAsync(p => p.Id == task.ProjectId);
        if (!projectExists)
        {
            throw new Exception("Ugyldig prosjekt-id.");
        }

        var userExists = await db.Users.AnyAsync(u => u.Id == task.AssignedUserId);
        if (!userExists)
        {
            throw new Exception("Ugyldig bruker-id.");
        }

        var statusExists = await db.Status.AnyAsync(s => s.Id == task.StatusId);
        if (!statusExists)
        {
            throw new Exception("Ugyldig status-id.");
        }

        await db.Tasks.AddAsync(task);
        await db.SaveChangesAsync();
    }

    public async Task<bool> Update(TaskItem task)
    {
        // Oppdaterer eksisterende oppgave
        var existing = await db.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
        if (existing == null)
        {
            return false;
        }

        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.DueDate = task.DueDate;

        var statusExists = await db.Status.AnyAsync(s => s.Id == task.StatusId);
        if (!statusExists)
        {
            throw new Exception("Ugyldig status-id.");
        }

        existing.StatusId = task.StatusId;

        var userExists = await db.Users.AnyAsync(u => u.Id == task.AssignedUserId);
        if (!userExists)
        {
            throw new Exception("Ugyldig bruker-id.");
        }

        existing.AssignedUserId = task.AssignedUserId;

        var projectExists = await db.Projects.AnyAsync(p => p.Id == task.ProjectId);
        if (!projectExists)
        {
            throw new Exception("Ugyldig prosjekt-id.");
        }

        existing.ProjectId = task.ProjectId;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(int id)
    {
        // Sletter oppgave hvis den finnes
        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (task == null)
        {
            return false;
        }

        db.Tasks.Remove(task);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStatus(int taskId, int statusId)
    {
        // Oppdaterer status på oppgaven
        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task == null)
        {
            return false;
        }

        var statusExists = await db.Status.AnyAsync(s => s.Id == statusId);
        if (!statusExists)
        {
            throw new Exception("Ugyldig status-id.");
        }

        task.StatusId = statusId;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignUser(int taskId, int userId)
    {
        // Tildeler oppgaven til en bruker
        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task == null)
        {
            return false;
        }

        var userExists = await db.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new Exception("Ugyldig bruker-id.");
        }

        task.AssignedUserId = userId;
        await db.SaveChangesAsync();
        return true;
    }
}
