using System.ComponentModel.DataAnnotations;

namespace ChatApp.Contracts.Messages.Requests;

public class CreateMessageRequestDto
{
    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;
}