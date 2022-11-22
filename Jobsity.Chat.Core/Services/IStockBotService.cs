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
    /// Tries to enqueue a message to the stock bot for processing.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="roomId"></param>
    /// <param name="connectionId"></param>
    public Task<bool> TryEnqueueAsync(string message, string roomId, string connectionId);
}