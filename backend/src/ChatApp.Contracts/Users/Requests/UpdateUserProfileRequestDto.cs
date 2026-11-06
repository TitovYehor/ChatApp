using System.ComponentModel.DataAnnotations;

namespace ChatApp.Contracts.Users.Requests;

/// <summary>
/// Contains the profile information that can be updated by the current user
/// </summary>
public class UpdateUserProfileRequestDto
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;
}