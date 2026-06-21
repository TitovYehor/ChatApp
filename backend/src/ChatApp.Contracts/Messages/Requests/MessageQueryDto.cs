namespace ChatApp.Contracts.Messages.Requests;

public class MessageQueryDto
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 50;
}