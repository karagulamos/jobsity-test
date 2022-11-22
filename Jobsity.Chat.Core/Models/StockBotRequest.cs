namespace Jobsity.Chat.Core.Models;

public class StockBotRequest
{
    public StockBotRequest(string stockCode, string roomId, string connectionId)
    {
        StockCode = stockCode;
        RoomId = roomId;
        ConnectionId = connectionId;
    }

    public string StockCode { get; }
    public string RoomId { get; }
    public string ConnectionId { get; }
}