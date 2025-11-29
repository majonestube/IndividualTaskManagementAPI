using MyShared.Models;
using TaskManagementAPI.Models.DTO;

namespace TaskManagementAPI.Services.UserServices;

public interface IUserService
{
    // Hent alle brukere
    Task<List<UserDto>> GetUsers();
    // Hent bruker etter id
    Task<UserDto?> GetUserById(string id);
    // Oppdater eksisterende bruker
    Task<UserDto?> Update(string id, UserUpdateDto dto, string userId);
    // Slett bruker
    Task<bool> Delete(string id, string userId);
    // Delete as admin
    Task<bool> DeleteAsAdmin(string id);
}