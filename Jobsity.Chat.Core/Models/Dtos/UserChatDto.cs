namespace Jobsity.Chat.Core.Models.Dtos;

public class UserChatDto
{
    public UserChatDto(string userId, string message, DateTime dateSent)
    {
        UserId = userId;
        Message = message;
        DateSent = dateSent;
    }

    public string UserId { get; }
    public string Message { get; }
    public DateTime DateSent { get; }

    public static explicit operator UserChatDto(UserChat chat)
    => new UserChatDto(chat.UserId, chat.Message, chat.DateCreated);
}