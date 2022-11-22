namespace Jobsity.Chat.Core.Models;

public class StockBotRequest
{
    public StockBotRequest(string correlationId, string stockCode)
    {
        CorrelationId = correlationId;
        StockCode = stockCode;
    }

    public string CorrelationId { get; }
    public string StockCode { get; }
}