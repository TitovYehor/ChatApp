using System.ComponentModel.DataAnnotations;

namespace ChatApp.Contracts.Users.Requests;

/// <summary>
/// Contains the current and new passwords submitted when changing a user's password
/// </summary>
public class ChangePasswordRequestDto
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}