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
                Status = t.Status.Name,
                DueDate = t.DueDate,
                ProjectName = t.Project.Name,
                AssignedUserName = t.AssignedUser.Username
            })
            .ToListAsync();

        return tasks;
    }

    public async Task<TaskItemDto?> GetById(int id)
    {
        // Henter oppgave etter id
        var task = await db.Tasks
            .AsNoTracking()
            .Include(t => t.Status)
            .Include(t => t.Project)
            .Include(t => t.AssignedUser)
            .FirstOrDefaultAsync(t => t.Id == id);

        return task == null ? null : TaskToDto(task);
    }

    public async Task Create(TaskItemCreateDto task)
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

        var newTaskItem = new TaskItem
        {
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            StatusId = task.StatusId,
            ProjectId = task.ProjectId,
            AssignedUserId = task.AssignedUserId,
        };

        await db.Tasks.AddAsync(newTaskItem);
        await db.SaveChangesAsync();
    }

    public async Task<bool> Update(int id, TaskItemCreateDto task)
    {
        // Oppdaterer eksisterende oppgave
        var existing = await db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
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

    public async Task<List<UserDto>> GetUsers(int taskId)
    {
        var projectId = await db.Tasks
            .Where(t => t.Id == taskId)
            .Select(t => t.ProjectId)
            .FirstOrDefaultAsync();
        
        var possibleUsers = await db.ProjectVisibility
            .AsNoTracking()
            .Where(pv => pv.ProjectId == projectId)
            .Select(pv => pv.UserId)
            .ToListAsync();

        var users = await db.Users
            .AsNoTracking()
            .Where(u => possibleUsers.Contains(u.Id))
            .Select(u => new UserDto
            {
                Email = u.Email,
                Username = u.Username,
            })
            .ToListAsync();
        return users;
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

    private static TaskItemDto TaskToDto(TaskItem task)
    {
        return new TaskItemDto
        {
            Title = task.Title,
            Description = task.Description,
            Status = task.Status?.Name ?? "Unknown status",
            DueDate = task.DueDate.Date,
            ProjectName = task.Project?.Name ?? "Unknown project name",
            AssignedUserName = task.AssignedUser?.Username ?? "Unknown user",
        };
    }
}
