using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public class ProjectService : IProjectService
{
    private readonly TaskManagementDbContext _db;

    public ProjectService(TaskManagementDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProjectDto>> GetProjectsForUser(int userId)
    {
        // Henter prosjekter for en bruker og mapper til DTO
        var projects = await _db.Projects
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
                TaskCount = p.Tasks.Count.ToString()
            })
            .ToListAsync();

        return projects;
    }

    public async Task<Project?> GetById(int id)
    {
        // Henter prosjekt etter id
        return await _db.Projects
            .AsNoTracking()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task Create(Project project)
    {
        // Oppretter nytt prosjekt etter enkel validering
        var userExists = await _db.Users.AnyAsync(u => u.Id == project.UserId);
        if (!userExists)
        {
            throw new Exception("Ugyldig bruker-id.");
        }

        await _db.Projects.AddAsync(project);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> Update(Project project)
    {
        // Oppdaterer eksisterende prosjekt
        var existing = await _db.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        if (existing == null)
        {
            return false;
        }

        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.UserId = project.UserId;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(int id)
    {
        // Sletter prosjekt hvis det finnes
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project == null)
        {
            return false;
        }

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        return true;
    }
}
