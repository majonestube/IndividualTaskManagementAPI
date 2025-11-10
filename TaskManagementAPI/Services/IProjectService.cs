using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public interface IProjectService
{
    // Hent alle prosjekter synlige/tilhørende en bruker
    Task<List<ProjectDto>> GetProjectsForUser(int userId);
    // Hent prosjekt etter id
    Task<Project?> GetById(int id);
    // Opprett nytt prosjekt basert på entiteten
    Task Create(Project project);
    // Oppdater eksisterende prosjekt basert på entiteten
    Task<bool> Update(Project project);
    // Slett prosjekt
    Task<bool> Delete(int id);
}


