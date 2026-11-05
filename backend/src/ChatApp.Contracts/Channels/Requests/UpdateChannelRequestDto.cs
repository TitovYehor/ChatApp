using System.ComponentModel.DataAnnotations;

namespace ChatApp.Contracts.Channels.Requests;

/// <summary>
/// Contains the values used to update an existing channel
/// </summary>
public class UpdateChannelRequestDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}