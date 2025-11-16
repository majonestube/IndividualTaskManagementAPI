using Microsoft.AspNetCore.Identity;
using TaskManagementAPI.Models.DTO;

namespace TaskManagementAPI.Services.UserServices;

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

    public async Task<UserDto?> Update(string id, UserUpdateDto dto, string userId)
    {
        if (userId != id)
        {
            throw new UnauthorizedAccessException("Bruker har ikke tilgang");
        }
        
        var user = await userManager.FindByIdAsync(id);
        
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

    public async Task<bool> Delete(string id, string userId)
    {
        if (userId != id)
        {
            throw new UnauthorizedAccessException("Bruker har ikke tilgang");
        }
        
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded;
    }
    
    public async Task<bool> DeleteAsAdmin(string id)
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
