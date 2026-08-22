using System.ComponentModel.DataAnnotations;

namespace ChatApp.Contracts.Channels.Requests;

public class CreateChannelRequestDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}