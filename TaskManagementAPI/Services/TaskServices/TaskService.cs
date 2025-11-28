using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShared.Models;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services.TaskServices;

public class TaskService(TaskManagementDbContext db, UserManager<IdentityUser> userManager) : ITaskService
{
    private readonly TaskManagementDbContext _db = db;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    public async Task<List<TaskItemDto>> GetTasksForProject(int projectId, string userId)
    {
        var canAccess = await CanAccessProject(projectId, userId);
        if (!canAccess)
        {
            throw new UnauthorizedAccessException("Bruker har ikke tilgang til prosjektet");
        }
        
        var tasks = await _db.Tasks
            .AsNoTracking()
            .Include(t => t.Status)
            .Include(t => t.Project)
            .Include(t => t.AssignedUser)
            .Where(t => t.ProjectId == projectId)
            .Select(t => new TaskItemDto
            {
                Id  = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status.Name,
                DueDate = t.DueDate,
                ProjectId = t.Project.Id,
                ProjectName = t.Project.Name,
                AssignedUserId = t.AssignedUser.Id,
                AssignedUserName = t.AssignedUser.UserName
            })
            .ToListAsync();

        return tasks;
    }

    public async Task<TaskItemDto?> GetById(int id)
    {
        var task = await _db.Tasks
            .AsNoTracking()
            .Include(t => t.Status)
            .Include(t => t.Project)
            .Include(t => t.AssignedUser)
            .FirstOrDefaultAsync(t => t.Id == id);

        return task == null ? null : TaskToDto(task);
    }

    public async Task Create(TaskItemCreateDto task, string userId)
    {
        var projectId = task.ProjectId;
        var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists) throw new BadHttpRequestException("Ugyldig prosjekt-id.");
        
        var canAccess = await CanAccessProject(projectId, userId);
        if (!canAccess)
        {
            throw new UnauthorizedAccessException("Bruker har ikke tilgang til prosjektet");
        }

        var statusExists = await _db.Status.AnyAsync(s => s.Id == task.StatusId);
        if (!statusExists) throw new Exception("Ugyldig status-id.");

        var newTaskItem = new TaskItem
        {
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            StatusId = task.StatusId,
            ProjectId = task.ProjectId,
            AssignedUserId = task.AssignedUserId
        };

        await _db.Tasks.AddAsync(newTaskItem);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> Update(int id, TaskItemCreateDto task, string userId)
    {
        var isOwner = await IsTaskOwner(id, userId);
        if (!isOwner)
        {
            throw new UnauthorizedAccessException("Bruker har ikke tilgang til oppgaven");
        }
        
        var existing = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (existing == null) return false;

        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.DueDate = task.DueDate;

        var statusExists = await _db.Status.AnyAsync(s => s.Id == task.StatusId);
        if (!statusExists) throw new BadHttpRequestException("Ugyldig status-id.");
        existing.StatusId = task.StatusId;

        var user = await _userManager.FindByIdAsync(task.AssignedUserId);
        if (user == null) throw new Exception("Ugyldig bruker-id.");
        existing.AssignedUserId = task.AssignedUserId;

        var projectExists = await _db.Projects.AnyAsync(p => p.Id == task.ProjectId);
        if (!projectExists) throw new BadHttpRequestException("Ugyldig prosjekt-id.");
        existing.ProjectId = task.ProjectId;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(int id, string userId)
    {
        var isOwner = await IsTaskOwner(id, userId);
        if (!isOwner)
        {
            throw new UnauthorizedAccessException("Bruker har ikke tilgang til oppgaven");
        }
        
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return false;

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStatus(int id, int statusId, string userId)
    {
        var isOwner = await IsTaskOwner(id, userId);
        if (!isOwner)
        {
            throw new UnauthorizedAccessException("Bruker har ikke tilgang til oppgaven");
        }
        
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return false;

        var statusExists = await _db.Status.AnyAsync(s => s.Id == statusId);
        if (!statusExists) throw new Exception("Ugyldig status-id.");

        task.StatusId = statusId;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<UserDto>> GetUsers(int taskId)
    {
        var projectId = await _db.Tasks
            .Where(t => t.Id == taskId)
            .Select(t => t.ProjectId)
            .FirstOrDefaultAsync();

        var possibleUserIds = await _db.ProjectVisibility
            .AsNoTracking()
            .Where(pv => pv.ProjectId == projectId)
            .Select(pv => pv.UserId)
            .ToListAsync();

        var users = new List<UserDto>();
        foreach (var userId in possibleUserIds)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                users.Add(new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName!,
                    Email = user.Email!
                });
            }
        }
        return users;
    }

    public async Task<bool> AssignUser(int taskId, string userId)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task == null) return false;

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new Exception("Ugyldig bruker-id.");

        task.AssignedUserId = userId;
        await _db.SaveChangesAsync();
        return true;
    }

    // Sjekker om bruker eier oppgaven
    public async Task<bool> IsTaskOwner(int taskId, string userId)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task == null) return false;
        return task.AssignedUserId == userId;
    }

    // Sjekker om bruker har tilgang til prosjektet
    public async Task<bool> CanAccessProject(int taskId, string userId)
    {
        var projectId = await _db.Tasks
            .Where(t => t.Id == taskId)
            .Select(t => t.ProjectId)
            .FirstOrDefaultAsync();
        if (projectId == null) return false;
        
        return await _db.ProjectVisibility
            .AsNoTracking()
            .AnyAsync(pv => pv.ProjectId == projectId && pv.UserId == userId);
    }

    private static TaskItemDto TaskToDto(TaskItem task)
    {
        return new TaskItemDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description ?? "",
            Status = task.Status?.Name ?? "Unknown status",
            DueDate = task.DueDate.Date,
            ProjectId = task.ProjectId,
            ProjectName = task.Project?.Name ?? "Unknown project name",
            AssignedUserId = task.AssignedUserId,
            AssignedUserName = task.AssignedUser?.UserName ?? "Unknown user"
        };
    }
}
