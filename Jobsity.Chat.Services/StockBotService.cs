
using System.Text.RegularExpressions;
using EasyNetQ;
using Jobsity.Chat.Core.Common;
using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Core.Services;

namespace Jobsity.Chat.Services;

public class StockBotService : IStockBotService
{
    private const string BotCommandPattern = @"\/stock\s*=\s*([\w.]+)";

    private readonly static Regex BotCommandRegex;
    private readonly IBus _bus;
    private readonly IStockTickerService _stockTickerService;

    static StockBotService()
    {
        BotCommandRegex = new Regex(BotCommandPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    public StockBotService(IBus bus, IStockTickerService stockTickerService)
    {
        _bus = bus;
        _stockTickerService = stockTickerService;
    }

    public async Task<bool> Enqueue(string correlationId, string message)
    {
        var match = BotCommandRegex.Match(message);

        if (!match.Success || match.Groups.Count < 2)
            return false;

        var request = new StockBotRequest(correlationId, match.Groups[1].Value);

        await _bus.PubSub.PublishAsync(request);

        await _bus.PubSub.SubscribeAsync(request.CorrelationId, async (StockBotRequest request) =>
        {
            var stockPrice = await _stockTickerService.GetStockPriceAsync(request.StockCode);

            if (stockPrice == null)
                return;

            var userChat = new UserChat(Constants.StockBotId, $"{stockPrice.Code} quote is ${stockPrice.Price} per share.", Guid.Parse(request.CorrelationId));

            // TODO: Send message to the user using SignalR and their ConnectionId (i.e. CorrelationId)
        });

        return true;
    }

    public bool IsBotCommand(string message) => message.Replace(" ", "").StartsWith("/stock=");
}