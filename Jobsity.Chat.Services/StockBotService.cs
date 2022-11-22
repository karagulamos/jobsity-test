
using System.Text.RegularExpressions;
using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Core.Services;

namespace Jobsity.Chat.Services;

public class StockBotService : IStockBotService
{
    private const string BotCommandPattern = @"\/stock\s*=\s*([\w.]+)";

    private readonly static Regex BotCommandRegex;

    static StockBotService()
    {
        BotCommandRegex = new Regex(BotCommandPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    public Task<bool> Enqueue(string correlationId, string message)
    {
        var match = BotCommandRegex.Match(message);

        if (!match.Success)
            return Task.FromResult(false);

        var request = new StockBotRequest(correlationId, match.Value);

        return Task.FromResult(true);
    }

    public bool IsBotCommand(string message) => message.Replace(" ", "").StartsWith("/stock=");

    private static void ConsumeRequests()
    {
    }
}