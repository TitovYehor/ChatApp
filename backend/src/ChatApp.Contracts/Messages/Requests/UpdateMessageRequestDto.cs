using System.ComponentModel.DataAnnotations;

namespace ChatApp.Contracts.Messages.Requests;

/// <summary>
/// Contains the updated content of an existing message
/// </summary>
public class UpdateMessageRequestDto
{
    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;
}