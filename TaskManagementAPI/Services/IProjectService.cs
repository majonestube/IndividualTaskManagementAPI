using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public interface IProjectService
{
    // Get visible projects
    Task<List<ProjectDto>> GetAllVisibleProjects(int userId);
    // Hent alle prosjekter synlige/tilhørende en bruker
    Task<List<ProjectDto>> GetProjectsForUser(int userId);
    // Hent prosjekt etter id
    Task<ProjectDto?> GetById(int id);
    // Opprett nytt prosjekt basert på entiteten
    Task<ProjectDto> Create(ProjectCreateDto project);
    // Oppdater eksisterende prosjekt basert på entiteten
    Task<bool> Update(int id, ProjectCreateDto project);
    // Slett prosjekt
    Task<bool> Delete(int id);
}


