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
        _logger.LogInformation($"Message from {userId} - {message}");

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

        if (_stockBot.FoundValidCommand(message))
        {
            _logger.LogInformation($"StockBot command found in message from {userId} - {message}");

            await Clients.Caller.ReceiveNewMessage(new UserChatDto(StockBotId, BotProcessingRequest, DateTime.Now));

            if (!await _stockBot.TryEnqueueAsync(message, roomId.ToString(), Context.ConnectionId))
            {
                await Clients.Caller.ReceiveNewMessage(new UserChatDto(StockBotId, BotUnableToProcessRequest, DateTime.Now));
            }

            return;
        }

        var newChat = new UserChat(userId, message, roomId, DateTime.Now);

        await _chats.AddAsync(newChat);

        await Clients.Group(roomId.ToString()).ReceiveNewMessage((UserChatDto)newChat);
    }
}