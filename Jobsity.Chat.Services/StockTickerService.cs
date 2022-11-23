using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Core.Models.Options;
using Jobsity.Chat.Core.Persistence;
using Jobsity.Chat.Core.Services;
using Microsoft.Extensions.Options;

namespace Jobsity.Chat.Services;

public class StockTickerService : IStockTickerService
{
    private readonly HttpClient _client;
    private readonly ICache _cache;
    private readonly StockTickerOptions _options;

    public StockTickerService(HttpClient client, ICache cache, IOptions<StockTickerOptions> options)
    {
        _client = client;
        _cache = cache;
        _options = options.Value;
    }

    public async Task<StockQuote?> GetQuoteAsync(string stockCode)
    {
        var stockQuote = await _cache.GetAsync<StockQuote>(stockCode);

        if (stockQuote != default)
            return stockQuote;

        var response = await _client.GetAsync($"?s={stockCode}&f=sd2t2ohlcv&h&e=csv");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error getting stock quote: {response.StatusCode}"); // IMPROV: Custom exception

        var content = await response.Content.ReadAsStringAsync();

        // The response is a CSV file with the following format:
        //
        // Symbol, Date, Time, Open, High, Low, Close, Volume
        // MSFT, 2021-03-26, 16:00:00, 237.0000, 237.0000, 236.0000, 236.0000, 0

        var lines = content.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2)
            throw new Exception("Invalid stock report format"); // IMPROV: Custom exception

        var values = lines[1].Split(",");

        if (values.Length < 8
            || !string.Equals(values[0], stockCode, StringComparison.OrdinalIgnoreCase)
            || !decimal.TryParse(values[6], out var price))
            throw new Exception("Error parsing stock price"); // IMPROV: Custom exception

        stockQuote = new StockQuote(stockCode, price);

        await _cache.SetAsync(stockCode, stockQuote, _options.StockQuoteCacheDuration);

        return stockQuote;
    }
}
