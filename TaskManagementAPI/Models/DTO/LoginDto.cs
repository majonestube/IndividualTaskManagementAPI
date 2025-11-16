using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models.DTO;

public class LoginDto
{
    [Required(ErrorMessage = "Brukernavn er påkrevd!")]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = "Passord er påkrevd!")]
    public string Password { get; set; } = string.Empty;
}