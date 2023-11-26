namespace Jobsity.Chat.Core.Models.Dtos;

public record UserChatDto(string UserId, string Message, DateTime DateSent)
{
    public static explicit operator UserChatDto(UserChat chat)
    => new(chat.UserId, chat.Message, chat.DateCreated);
}
