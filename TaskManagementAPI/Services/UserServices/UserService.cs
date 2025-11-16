using Microsoft.AspNetCore.Identity;
using TaskManagementAPI.Models.DTO;

namespace TaskManagementAPI.Services.UserServices;

public class UserService(UserManager<IdentityUser> userManager) : IUserService
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    public async Task<List<UserDto>> GetUsers()
    {
        var users = _userManager.Users
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
        var user = await _userManager.FindByIdAsync(id);
        return user == null ? null : UserToDto(user);
    }

    // Oppdatere bruker, krever at man selv er brukeren som skal oppdateres
    public async Task<UserDto?> Update(string id, UserUpdateDto dto, string userId)
    {
        if (userId != id)
        {
            throw new UnauthorizedAccessException("Bruker har ikke tilgang");
        }
        
        var user = await _userManager.FindByIdAsync(id);
        
        user.UserName = dto.Username;
        user.Email = dto.Email;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            throw new Exception($"Kunne ikke oppdatere bruker: {errors}");
        }

        if (!string.IsNullOrEmpty(dto.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);

            if (!passwordResult.Succeeded)
            {
                var errors = string.Join(", ", passwordResult.Errors.Select(e => e.Description));
                throw new Exception($"Kunne ikke oppdatere passord: {errors}");
            }
        }

        return UserToDto(user);
    }

    // Slette bruker, krever at man selv er brukeren som skal slettes
    public async Task<bool> Delete(string id, string userId)
    {
        if (userId != id)
        {
            throw new UnauthorizedAccessException("Bruker har ikke tilgang");
        }
        
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
    
    // Slette en valgt bruker uten å være brukeren selv
    public async Task<bool> DeleteAsAdmin(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.DeleteAsync(user);
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
