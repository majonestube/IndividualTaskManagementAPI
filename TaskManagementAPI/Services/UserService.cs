using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public class UserService(TaskManagementDbContext db) : IUserService
{
    public async Task<List<UserDto>> GetUsers()
    {
        return await db.Users
            .Select(u => new UserDto { Username = u.Username, Email = u.Email })
            .ToListAsync();
    }

    public async Task<User?> GetUserById(int id)
    {
        return await db.Users
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task Create(UserCreateDto dto)
    {
        var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (existingUser == null)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password // Consider hashing before saving
            };
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Username already exists");
        }
    }
    
    public async Task Update(int id, UserUpdateDto dto)
    {
        var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (existingUser == null) throw new Exception("User not found");

        existingUser.Username = dto.Username;
        existingUser.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.Password))
            existingUser.Password = dto.Password; // TODO Hashing

        await db.SaveChangesAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (existingUser == null)
        {
            return false;
        }
        
        db.Users.Remove(existingUser);
        await db.SaveChangesAsync();
        return true;
    }

    public UserDto UserToDto(User user)
    {
        return new UserDto
        {   
            Username = user.Username,
            Email = user.Email
        };
    }
}