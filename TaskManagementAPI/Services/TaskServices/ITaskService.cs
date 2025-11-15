using TaskManagementAPI.Models.DTO;

namespace TaskManagementAPI.Services.TaskServices;

public interface ITaskService
{
    // Hent alle oppgaver for et prosjekt
    Task<List<TaskItemDto>> GetTasksForProject(int projectId, string userId);
    // Hent oppgave etter id
    Task<TaskItemDto?> GetById(int id);
    // Opprett ny oppgave
    Task Create(TaskItemCreateDto task, string userId);
    // Oppdater eksisterende oppgave
    Task<bool> Update(int id, TaskItemCreateDto task, string userId);
    // Slett oppgave
    Task<bool> Delete(int id, string userId);
    // Oppdater status for oppgave
    Task<bool> UpdateStatus(int taskId, int statusId, string userId);
    // Get users with access to project
    Task<List<UserDto>> GetUsers(int projectId);
    // Tildel oppgave til bruker
    Task<bool> AssignUser(int taskId, string userId);
}


