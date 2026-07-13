using ChatApp.Contracts.Messages.Responses;

namespace ChatApp.SignalRTester.Application.State;

public class MessageCache
{
    private readonly List<MessageResponseDto> _messages = [];

    public IReadOnlyList<MessageResponseDto> Messages => _messages;

    public void Replace(
        IEnumerable<MessageResponseDto> messages)
    {
        _messages.Clear();

        _messages.AddRange(messages);
    }

    public void Add(
        MessageResponseDto message)
    {
        if (_messages.Any(x => x.Id == message.Id))
        {
            return;
        }

        _messages.Add(message);

        _messages.Sort((x, y) => x.CreatedAt.CompareTo(y.CreatedAt));
    }

    public bool Remove(
        Guid messageId)
    {
        var message = _messages.FirstOrDefault(
            x => x.Id == messageId);

        if (message == null)
        {
            return false;
        }

        _messages.Remove(message);

        return true;
    }

    public void Update(
        MessageResponseDto message)
    {
        var index = _messages.FindIndex(
        x => x.Id == message.Id);

        if (index == -1)
        {
            _messages.Add(message);

            return;
        }

        _messages[index] = message;
    }
}