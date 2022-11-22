namespace Jobsity.Chat.Core.Models;

public class ChatRoom
{
    public ChatRoom(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; }
}