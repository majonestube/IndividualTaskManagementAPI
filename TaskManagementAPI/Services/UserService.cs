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

    public async Task Save(User user)
    {
        var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        if (existingUser != null)
        {
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
        }
        else
        {
            await db.Users.AddAsync(user);
        }
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