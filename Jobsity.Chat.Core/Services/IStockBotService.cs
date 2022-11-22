namespace Jobsity.Chat.Core.Services;

public interface IStockBotService
{
    /// <summary>
    /// Checks if the message is a bot command.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public bool FoundValidCommand(string message);

    /// <summary>
    /// Processes the bot command message.
    /// </summary>
    /// <param name="correlationId"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task<bool> TryEnqueueAsync(string message, string correlationId);
}