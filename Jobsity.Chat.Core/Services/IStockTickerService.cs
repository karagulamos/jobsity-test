using Jobsity.Chat.Core.Models;

namespace Jobsity.Chat.Core.Services;

public interface IStockTickerService
{
    /// <summary>
    /// Gets the latest quote for the specified stock code.
    /// </summary>
    /// <param name="stockCode"></param>
    /// <returns></returns>
    Task<StockQuote> GetQuoteAsync(string stockCode);
}