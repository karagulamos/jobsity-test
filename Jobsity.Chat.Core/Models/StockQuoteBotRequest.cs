namespace Jobsity.Chat.Core.Models;

public record StockQuoteBotRequest(string StockCode, string RoomId, string ConnectionId);