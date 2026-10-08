using System.ComponentModel.DataAnnotations;

namespace ChatApp.Contracts.Users.Requests;

public class UpdateUserProfileRequestDto
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;
}