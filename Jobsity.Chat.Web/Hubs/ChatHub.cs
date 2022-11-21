using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Core.Persistence;
using Microsoft.AspNetCore.SignalR;

namespace Jobsity.Chat.Web.Hubs;

public class ChatHub : Hub
{
    private const string ReceiveNewMessage = nameof(ReceiveNewMessage);

    private readonly ILogger<ChatHub> _logger;
    private readonly IChatRepository _chats;

    public ChatHub(ILogger<ChatHub> logger, IChatRepository chats)
    {
        _logger = logger;
        _chats = chats;
    }

    public async Task SendNewMessage(string userId, string message)
    {
        _logger.LogInformation($"Message from {userId} - {message}");

        var newChat = new UserChat(userId, message);

        await _chats.AddAsync(newChat);

        await Clients.All.SendAsync(ReceiveNewMessage, (UserChatDto) newChat);
    }
}