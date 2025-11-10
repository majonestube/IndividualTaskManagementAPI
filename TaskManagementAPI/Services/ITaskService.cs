using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public interface ITaskService
{
    // Hent alle oppgaver for et prosjekt
    Task<List<TaskItemDto>> GetTasksForProject(int projectId);
    // Hent oppgave etter id
    Task<TaskItem?> GetById(int id);
    // Opprett ny oppgave
    Task Create(TaskItem task);
    // Oppdater eksisterende oppgave
    Task<bool> Update(TaskItem task);
    // Slett oppgave
    Task<bool> Delete(int id);
    // Oppdater status for oppgave
    Task<bool> UpdateStatus(int taskId, int statusId);
    // Tildel oppgave til bruker
    Task<bool> AssignUser(int taskId, int userId);
}


