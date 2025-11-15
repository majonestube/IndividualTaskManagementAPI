using TaskManagementAPI.Models.DTO;

namespace TaskManagementAPI.Services.ProjectServices;

public interface IProjectService
{
    // Get visible projects
    Task<List<ProjectDto>> GetAllVisibleProjects(string userId);
    // Hent alle prosjekter synlige/tilhørende en bruker
    Task<List<ProjectDto>> GetProjectsForUser(string userId);
    // Hent prosjekt etter id
    Task<ProjectDto?> GetById(int id);
    // Opprett nytt prosjekt basert på entiteten
    Task<ProjectDto> Create(ProjectCreateDto project);
    // Oppdater eksisterende prosjekt basert på entiteten
    Task<bool> Update(int id, ProjectCreateDto project, string userId);
    // Slett prosjekt
    Task<bool> Delete(int id, string userId);
}


