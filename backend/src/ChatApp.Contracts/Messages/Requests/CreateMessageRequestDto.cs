using System.ComponentModel.DataAnnotations;

namespace ChatApp.Contracts.Messages.Requests;

/// <summary>
/// Contains the content of a new channel message
/// </summary>
public class CreateMessageRequestDto
{
    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;
}