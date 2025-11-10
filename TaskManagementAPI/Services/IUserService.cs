using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public interface IUserService
{
    // Hent alle brukere
    Task<List<UserDto>> GetUsers();
    // Hent bruker etter id
    Task<UserDto?> GetUserById(int id);
    // Opprett ny bruker
    Task<UserDto> Create(UserCreateDto dto);
    // Oppdater eksisterende bruker
    Task<UserDto?> Update(int id, UserUpdateDto dto);
    // Slett bruker
    Task<bool> Delete(int id);
}