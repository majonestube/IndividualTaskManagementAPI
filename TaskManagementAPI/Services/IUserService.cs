using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public interface IUserService
{
    Task<List<UserDto>> GetUsers();
    Task<User?> GetUserById(int id);
    Task Create(UserCreateDto dto);
    Task Update(int id, UserUpdateDto dto);
    Task<bool> Delete(int id);
}