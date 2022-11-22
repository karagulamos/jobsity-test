namespace Jobsity.Chat.Core.Models;

public class UserChat
{
    public UserChat(string userId, string message, Guid roomId)
    {
        UserId = userId;
        Message = message;
        RoomId = roomId;
        DateCreated = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public Guid RoomId { get; }
    public string UserId { get; }
    public string Message { get; }
    public DateTime DateCreated { get; }

    public ChatRoom ChatRoom { get; set; }
}