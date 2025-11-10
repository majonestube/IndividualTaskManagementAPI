using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public class ProjectService(TaskManagementDbContext db) : IProjectService
{
    public async Task<List<ProjectDto>> GetProjectsForUser(int userId)
    {
        // Henter prosjekter for en bruker og mapper til DTO
        var projects = await db.Projects
            .AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.Tasks)
            .Where(p => p.UserId == userId)
            .Select(p => new ProjectDto
            {
                Name = p.Name,
                Description = p.Description,
                Created = p.Created,
                Username = p.User.Username,
                TaskCount = p.Tasks.Count()
            })
            .ToListAsync();

        return projects;
    }

    public async Task<Project?> GetById(int id)
    {
        // Henter prosjekt etter id
        return await db.Projects
            .AsNoTracking()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ProjectDto> Create(ProjectCreateDto project)
    {
        // Oppretter nytt prosjekt etter enkel validering
        var userExists = await db.Users.AnyAsync(u => u.Id == project.UserId);
        if (!userExists)
        {
            throw new Exception("Ugyldig bruker-id.");
        }

        var newProject = new Project
        {
            Name = project.Name,
            Description = project.Description,
            UserId = project.UserId,
        };

        await db.Projects.AddAsync(newProject);
        await db.SaveChangesAsync();
        
        return ProjectToDto(newProject);
    }

    public async Task<bool> Update(Project project)
    {
        // Oppdaterer eksisterende prosjekt
        var existing = await db.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        if (existing == null)
        {
            return false;
        }

        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.UserId = project.UserId;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(int id)
    {
        // Sletter prosjekt hvis det finnes
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project == null)
        {
            return false;
        }

        db.Projects.Remove(project);
        await db.SaveChangesAsync();
        return true;
    }

    private static ProjectDto ProjectToDto(Project project)
    {
        return new ProjectDto
        {
            Name = project.Name,
            Description = project.Description,
            Created = project.Created,
            Username = project.User.Username,
            TaskCount = project.Tasks.Count
        };
    }
}
