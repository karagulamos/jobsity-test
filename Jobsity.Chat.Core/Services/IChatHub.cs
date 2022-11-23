using Jobsity.Chat.Core.Models.Dtos;

namespace Jobsity.Chat.Core.Services;

public interface IChatHub
{
    Task ReceiveNewMessage(UserChatDto chat);
}