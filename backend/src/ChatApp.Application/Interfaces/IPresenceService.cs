namespace ChatApp.Application.Interfaces;

public interface IPresenceService
{
    Task UserConnectedAsync(
        Guid userId,
        string connectionId);

    Task UserDisconnectedAsync(
        Guid userId,
        string connectionId);
}