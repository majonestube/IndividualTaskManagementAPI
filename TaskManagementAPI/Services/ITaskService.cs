using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public interface ITaskService
{
    // Hent alle oppgaver for et prosjekt
    Task<List<TaskItemDto>> GetTasksForProject(int projectId);
    // Hent oppgave etter id
    Task<TaskItemDto?> GetById(int id);
    // Opprett ny oppgave
    Task Create(TaskItemCreateDto task);
    // Oppdater eksisterende oppgave
    Task<bool> Update(int id, TaskItemCreateDto task);
    // Slett oppgave
    Task<bool> Delete(int id);
    // Oppdater status for oppgave
    Task<bool> UpdateStatus(int taskId, int statusId);
    // Get users with access to project
    Task<List<UserDto>> GetUsers(int projectId);
    // Tildel oppgave til bruker
    Task<bool> AssignUser(int taskId, string userId);
}


