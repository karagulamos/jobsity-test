using EasyNetQ;
using Jobsity.Chat.Core.Common;
using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Core.Models.Dtos;
using Jobsity.Chat.Core.Services;
using Jobsity.Chat.Services.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Jobsity.Chat.Services;

public class StockQuoteBotService : IStockQuoteBotService
{
    private readonly ILogger<ChatHub> _logger;
    private readonly IBus _bus;
    private readonly IStockTickerService _stockTicker;
    private readonly IHubContext<ChatHub, IChatHub> _hubContext;

    public StockQuoteBotService(ILogger<ChatHub> logger, IBus bus, IStockTickerService stockTicker, IHubContext<ChatHub, IChatHub> hubContext)
    {
        _logger = logger;
        _bus = bus;
        _stockTicker = stockTicker;
        _hubContext = hubContext;
    }

    public bool FoundValidCommand(string message) => message.Replace(" ", "").StartsWith("/stock=");

    public async Task<bool> TryEnqueueAsync(string message, string correlationId)
    {
        var tokens = message.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length != 2)
            return false;

        var request = new StockQuoteBotRequest(tokens[1].Trim(), correlationId);

        await _bus.PubSub.PublishAsync(request);

        await _bus.PubSub.SubscribeAsync(string.Empty, OnBotRequestTurnAsync);

        return true;
    }

    private Func<StockQuoteBotRequest, Task> OnBotRequestTurnAsync =>
    async (StockQuoteBotRequest request) =>
    {
        try
        {
            _logger.LogDebug($"StockBot processing request: {request}");

            var stockQuote = await _stockTicker.GetQuoteAsync(request.StockCode);

            var userChat = new UserChatDto(Constants.StockBotId, $"{stockQuote.Code.ToUpper()} quote is ${stockQuote.Price} per share.", DateTime.Now);
            await _hubContext.Clients.Client(request.CorrelationId).ReceiveNewMessage(userChat);

            _logger.LogInformation($"StockBot processed request: {request}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing stock bot request");

            var userChat = new UserChatDto(Constants.StockBotId, Constants.BotUnableToProcessRequest, DateTime.Now);
            await _hubContext.Clients.Client(request.CorrelationId).ReceiveNewMessage(userChat);
        }
    };
}