namespace Jobsity.Chat.Core.Models;

public record UserChat(string UserId, string Message, Guid RoomId, DateTime DateCreated)
{
    public Guid Id { get; }
    public ChatRoom ChatRoom { get; } = null!;
}