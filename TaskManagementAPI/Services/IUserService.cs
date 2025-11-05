using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public interface IUserService
{
    Task<List<UserDto>> GetUsers();
    Task<User?> GetUserById(int id);
    Task Save(User user);
    Task<bool> Delete(int id);
}