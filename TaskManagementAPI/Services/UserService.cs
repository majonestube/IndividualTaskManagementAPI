using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services;

public class UserService : IUserService
{
    private readonly TaskManagementDbContext _db;

    public UserService(TaskManagementDbContext db)
    {
        _db = db;
    }

    public async Task<List<UserDto>> GetUsers()
    {
        var users = await _db.Users
            .AsNoTracking()
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email
            })
            .ToListAsync();

        return users;
    }

    public async Task<UserDto?> GetUserById(int id)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        return user == null ? null : UserToDto(user);
    }

    public async Task<UserDto> Create(UserCreateDto dto)
    {
        var existingUser = await _db.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == dto.Username.ToLower());

        if (existingUser != null)
        {
            throw new Exception("Brukernavn finnes allerede."); // 409: Konflikt
        }

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = dto.Password // TODO: Hash passord før lagring
        };

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        return UserToDto(user);
    }

    public async Task<UserDto?> Update(int id, UserUpdateDto dto)
    {
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (existingUser == null)
        {
            return null;
        }

        existingUser.Username = dto.Username;
        existingUser.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.Password))
        {
            existingUser.Password = dto.Password; // TODO: Hash passord før lagring
        }

        await _db.SaveChangesAsync();

        return UserToDto(existingUser);
    }

    public async Task<bool> Delete(int id)
    {
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (existingUser == null)
        {
            return false;
        }

        _db.Users.Remove(existingUser);
        await _db.SaveChangesAsync();
        return true;
    }

    private static UserDto UserToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }
}