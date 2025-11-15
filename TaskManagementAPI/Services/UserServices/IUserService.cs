using TaskManagementAPI.Models.DTO;

namespace TaskManagementAPI.Services.UserServices;

public interface IUserService
{
    // Hent alle brukere
    Task<List<UserDto>> GetUsers();
    // Hent bruker etter id
    Task<UserDto?> GetUserById(string id);
    // Opprett ny bruker
    Task<UserDto> Create(UserCreateDto dto);
    // Oppdater eksisterende bruker
    Task<UserDto?> Update(string id, UserUpdateDto dto);
    // Slett bruker
    Task<bool> Delete(string id);
}