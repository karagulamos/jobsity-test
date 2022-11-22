using Jobsity.Chat.Core.Models;

namespace Jobsity.Chat.Core.Services;

public interface IStockBotService
{
    public bool IsBotCommand(string message);
    public Task<bool> Enqueue(string correlationId, string message);
}