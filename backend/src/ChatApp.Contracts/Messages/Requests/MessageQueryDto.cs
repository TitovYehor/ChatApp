using System.ComponentModel.DataAnnotations;

namespace ChatApp.Contracts.Messages.Requests;

public class MessageQueryDto
{
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 50;

    public string? Search { get; set; }
}