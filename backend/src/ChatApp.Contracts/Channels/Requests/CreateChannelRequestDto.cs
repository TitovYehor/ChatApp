using System.ComponentModel.DataAnnotations;

namespace ChatApp.Contracts.Channels.Requests;

/// <summary>
/// Contains the information required to create a channel
/// </summary>
public class CreateChannelRequestDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}