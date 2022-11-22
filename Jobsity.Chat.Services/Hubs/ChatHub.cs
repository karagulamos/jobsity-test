using Jobsity.Chat.Core.Common;
using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Core.Models.Dtos;
using Jobsity.Chat.Core.Persistence;
using Jobsity.Chat.Core.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Jobsity.Chat.Services.Hubs;

public class ChatHub : Hub
{
    private const string ReceiveNewMessage = nameof(ReceiveNewMessage);
    private const string BotBadCommand = "Unable to process your request. Please check the command and retry.";
    private const string BotProcessing = "Processing your request. Please wait...";

    private readonly ILogger<ChatHub> _logger;
    private readonly IChatRepository _chats;
    private readonly IStockBotService _stockBot;

    public ChatHub(ILogger<ChatHub> logger, IChatRepository chats, IStockBotService stockBot)
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

            var response = await _stockBot.TryEnqueueAsync(message, roomId.ToString(), Context.ConnectionId) ? BotProcessing : BotBadCommand;

            var botResponse = new UserChatDto(Constants.StockBotId, response, DateTime.Now);

            await Clients.Caller.SendAsync(ReceiveNewMessage, botResponse);
            return;
        }

        var newChat = new UserChat(userId, message, roomId);

        await _chats.AddAsync(newChat);
        await Clients.Group(roomId.ToString()).SendAsync(ReceiveNewMessage, (UserChatDto)newChat);
    }
}