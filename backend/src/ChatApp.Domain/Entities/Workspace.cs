namespace ChatApp.Domain.Entities;

public class Workspace
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<WorkspaceMember> Members { get; set; }
        = new List<WorkspaceMember>();

    public ICollection<Channel> Channels { get; set; }
        = new List<Channel>();
}