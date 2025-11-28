using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShared.Models;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services.ProjectServices;

public class ProjectService(TaskManagementDbContext db, UserManager<IdentityUser> userManager) : IProjectService
{
    private readonly TaskManagementDbContext _db = db;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    public async Task<List<ProjectDto>> GetAllProjects()
    {
        var projects = await _db.Projects
            .AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.Tasks)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Created = p.Created,
                Username = p.User.UserName,
                TaskCount = p.Tasks.Count()
            })
            .ToListAsync();

        return projects;
    }
    public async Task<List<ProjectDto>> GetAllVisibleProjects(string userId)
    {
        var visibleProjectIds = await _db.ProjectVisibility
            .AsNoTracking()
            .Where(pv => pv.UserId == userId)
            .Select(pv => pv.ProjectId)
            .ToListAsync();
        
        var projects = await _db.Projects
            .AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.Tasks)
            .Where(p => visibleProjectIds.Contains(p.Id))
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Created = p.Created,
                Username = p.User.UserName,
                TaskCount = p.Tasks.Count(),
                UnreadNotificationsCount = p.Notifications.Count(n => !n.IsRead && n.UserId == userId)
            })
            .ToListAsync();
        
        return projects;
    }

    public async Task<List<ProjectDto>> GetProjectsForUser(string userId)
    {
        // Henter prosjekter for en bruker og mapper til DTO
        var projects = await _db.Projects
            .AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.Tasks)
            .Where(p => p.UserId == userId)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Created = p.Created,
                Username = p.User.UserName,
                TaskCount = p.Tasks.Count()
            })
            .ToListAsync();

        return projects;
    }

    public async Task<ProjectDto?> GetById(int id)
    {
        // Henter prosjekt etter id
        var project = await _db.Projects
            .AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        return project == null ? null : ProjectToDto(project);
    }

    public async Task<ProjectDto> Create(ProjectCreateDto project)
    {
        // Oppretter nytt prosjekt
        var userExists = await _db.Users.AnyAsync(u => u.Id == project.UserId);
        if (!userExists)
        {
            throw new UnauthorizedAccessException("Ugyldig bruker-id.");
        }

        var newProject = new Project
        {
            Name = project.Name,
            Description = project.Description,
            UserId = project.UserId,
        };

        await _db.Projects.AddAsync(newProject);
        await _db.SaveChangesAsync();
    
        // Opprett ProjectVisibility for eieren
        var projectVisibility = new ProjectVisibility
        {
            ProjectId = newProject.Id,
            UserId = project.UserId
        };
        
        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        
        // Gjør synlig for admin brukere
        foreach (var admin in admins)
        {
            await _db.ProjectVisibility.AddAsync(new ProjectVisibility
            {
                ProjectId = newProject.Id,
                UserId = admin.Id
            });
        }
    
        await _db.ProjectVisibility.AddAsync(projectVisibility);
        
        await _db.SaveChangesAsync();
    
        return ProjectToDto(newProject);
    }

    public async Task<bool> Update(int id, ProjectCreateDto project, string userId)
    {
        // Oppdaterer eksisterende prosjekt
        var existing = await _db.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (existing == null)
        {
            return false;
        }

        if (existing.UserId != userId)
        {
            throw new UnauthorizedAccessException();
        }
        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.UserId = project.UserId;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(int id, string userId)
    {
        // Sletter prosjekt hvis det finnes
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project == null)
        {
            return false;
        }

        if (project.UserId != userId)
        {
            throw new UnauthorizedAccessException();
        }
        
        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        return true;
    }

    // Deler prosjekt med gitt bruker
    public async Task<bool> ShareProject(int projectId, string ownerUserId, string sharedUserId)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null)
        {
            return false;
        }
        
        if (project.UserId != ownerUserId)
            throw new UnauthorizedAccessException("Kun prosjekteier kan dele prosjektet.");
        
        var userExists = await _db.Users.AnyAsync(u => u.Id == ownerUserId);
        if (!userExists)
        {
            throw new UnauthorizedAccessException("Ugyldig bruker-id.");
        }
        
        var alreadyVisible = await _db.ProjectVisibility
            .AnyAsync(pv => pv.ProjectId == projectId && pv.UserId == sharedUserId);
        if (alreadyVisible)
        {
            return true;
        }

        var visibility = new ProjectVisibility
        {
            ProjectId = projectId,
            UserId = sharedUserId
        };
        
        await _db.ProjectVisibility.AddAsync(visibility);
        await _db.SaveChangesAsync();
        
        return true;
    }

    private static ProjectDto ProjectToDto(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Created = project.Created,
            Username = project.User?.UserName ?? "Unknown",
            TaskCount = project.Tasks?.Count ?? 0
        };
    }
}
