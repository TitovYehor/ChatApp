using ChatApp.Contracts.Realtime;

namespace ChatApp.SignalRTester.Application.State;

public class OnlineUsersCache
{
    private readonly Dictionary<Guid, UserPresenceChangedResponseDto> _users = [];

    public void UserChanged(UserPresenceChangedResponseDto user)
    {
        if (user.IsOnline)
        {
            _users[user.UserId] = user;
        }
        else
        {
            _users.Remove(user.UserId);
        }
    }

    public IReadOnlyCollection<UserPresenceChangedResponseDto> GetAll()
    {
        return _users.Values
            .OrderBy(x => x.Username)
            .ToList();
    }

    public void SetUsers(
        IEnumerable<OnlineUserResponseDto> users)
    {
        _users.Clear();

        foreach (var user in users)
        {
            _users[user.UserId] =
                new UserPresenceChangedResponseDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    IsOnline = true
                };
        }
    }

    public void Clear()
    {
        _users.Clear();
    }
}