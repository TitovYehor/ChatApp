namespace ChatApp.Contracts.Channels.Requests;

/// <summary>
/// Contains the optional search query used to filter channels
/// </summary>
public class ChannelSearchRequestDto
{
    public string? Query { get; set; }
}