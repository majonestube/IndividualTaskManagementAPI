using Microsoft.AspNetCore.Identity;
using TaskManagementAPI.Models.DTO;

namespace TaskManagementAPI.Services;

public class UserService(UserManager<IdentityUser> userManager) : IUserService
{
    public async Task<List<UserDto>> GetUsers()
    {
        var users = userManager.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.UserName!,
                Email = u.Email!
            })
            .ToList();

        return users;
    }

    public async Task<UserDto?> GetUserById(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        return user == null ? null : UserToDto(user);
    }

    public async Task<UserDto> Create(UserCreateDto dto)
    {
        var existingUser = await userManager.FindByNameAsync(dto.Username);
        if (existingUser != null)
        {
            throw new Exception("Brukernavn finnes allerede.");
        }

        var user = new IdentityUser
        {
            UserName = dto.Username,
            Email = dto.Email
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Kunne ikke opprette bruker: {errors}");
        }

        return UserToDto(user);
    }

    public async Task<UserDto?> Update(string id, UserUpdateDto dto)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        user.UserName = dto.Username;
        user.Email = dto.Email;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            throw new Exception($"Kunne ikke oppdatere bruker: {errors}");
        }

        if (!string.IsNullOrEmpty(dto.Password))
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await userManager.ResetPasswordAsync(user, token, dto.Password);

            if (!passwordResult.Succeeded)
            {
                var errors = string.Join(", ", passwordResult.Errors.Select(e => e.Description));
                throw new Exception($"Kunne ikke oppdatere passord: {errors}");
            }
        }

        return UserToDto(user);
    }

    public async Task<bool> Delete(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    private static UserDto UserToDto(IdentityUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.UserName!,
            Email = user.Email!
        };
    }
}
