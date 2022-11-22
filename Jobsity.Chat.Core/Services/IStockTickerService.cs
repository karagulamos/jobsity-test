using Jobsity.Chat.Core.Models;

namespace Jobsity.Chat.Core.Services;

public interface IStockTickerService
{
    /// <summary>
    /// Gets the latest stock prices for the specified stock.
    /// </summary>
    /// <param name="stockCode"></param>
    /// <returns></returns>
    Task<StockPrice?> GetStockPriceAsync(string stockCode);
}