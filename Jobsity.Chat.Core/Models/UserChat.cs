namespace Jobsity.Chat.Core.Models;

public class UserChat
{
    public UserChat(string userId, string message)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Message = message;
        DateCreated = DateTime.Now;
    }

    public Guid Id { get; }
    public string UserId { get; }
    public string Message { get; }
    public DateTime DateCreated { get; }
}