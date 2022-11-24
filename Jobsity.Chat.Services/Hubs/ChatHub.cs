using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Core.Models.Dtos;
using Jobsity.Chat.Core.Persistence;
using Jobsity.Chat.Core.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using static Jobsity.Chat.Core.Common.Constants;

namespace Jobsity.Chat.Services.Hubs;

public class ChatHub : Hub<IChatHub>
{
    private readonly ILogger<ChatHub> _logger;
    private readonly IChatRepository _chats;
    private readonly IStockQuoteBotService _stockBot;

    public ChatHub(ILogger<ChatHub> logger, IChatRepository chats, IStockQuoteBotService stockBot)
    {
        _logger = logger;
        _chats = chats;
        _stockBot = stockBot;
    }

    public async Task SendNewMessage(Guid roomId, string userId, string message)
    {
        _logger.LogInformation($"Received message from {userId} in room {roomId}.");

        // We keep this fallback in case calling JoinRoom by the client fails. However,
        // users risk missing a few initial chats from other users until they refresh
        // and/or send a few messages of their own.
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

        if (_stockBot.FoundValidCommand(message))
        {
            _logger.LogInformation($"StockBot command found: {message}.");

            await Clients.Caller.ReceiveNewMessage(new UserChatDto(StockBotId, BotProcessingRequest, DateTime.Now));

            if (!await _stockBot.TryEnqueueAsync(message, Context.ConnectionId))
            {
                _logger.LogWarning($"StockBot command could not be enqueued: {message}.");

                await Clients.Caller.ReceiveNewMessage(new UserChatDto(StockBotId, BotUnableToProcessRequest, DateTime.Now));
            }

            return;
        }

        _logger.LogDebug($"Saving message from {userId} in room {roomId}.");

        var newChat = new UserChat(userId, message, roomId, DateTime.Now);

        await _chats.AddAsync(newChat);

        _logger.LogDebug($"Publishing message from {userId} in room {roomId}.");

        await Clients.Group(roomId.ToString()).ReceiveNewMessage((UserChatDto)newChat);

        _logger.LogInformation($"Message from {userId} in room {roomId} published.");
    }

    public async Task JoinRoom(Guid roomId)
    {
        _logger.LogInformation($"User {Context.ConnectionId} joined room {roomId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
    }

    public async Task GetChatsSince(DateTime lastReadAt, Guid roomId)
    {
        _logger.LogInformation($"User {Context.ConnectionId} requested chats since {lastReadAt} in room {roomId}");

        var offlineChats = await _chats.GetSinceAsync(lastReadAt, roomId);

        foreach (var chat in offlineChats)
        {
            await Clients.Caller.ReceiveNewMessage((UserChatDto)chat);
        }

        _logger.LogInformation($"User {Context.ConnectionId} received {offlineChats.Length} chats since {lastReadAt} in room {roomId}");
    }
}