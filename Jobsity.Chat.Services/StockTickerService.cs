using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Core.Services;

namespace Jobsity.Chat.Services;

public class StockTickerService : IStockTickerService
{
    private readonly HttpClient _client;

    public StockTickerService(HttpClient client) => _client = client;

    public async Task<StockPrice?> GetStockPriceAsync(string stockCode)
    {
        var response = await _client.GetAsync($"?s={stockCode}&f=sd2t2ohlcv&h&e=csv");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error getting stock price: {response.StatusCode}");

        var content = await response.Content.ReadAsStringAsync();

        // The response is a CSV file with the following format:
        //
        // Symbol, Date, Time, Open, High, Low, Close, Volume
        // MSFT, 2021-03-26, 16:00:00, 237.0000, 237.0000, 236.0000, 236.0000, 0

        var lines = content.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2)
            throw new Exception("Invalid stock report format");

        var values = lines[1].Split(",");

        if (values.Length < 8
            || !string.Equals(values[0], stockCode, StringComparison.OrdinalIgnoreCase)
            || !decimal.TryParse(values[6], out var price))
            throw new Exception("Error parsing stock price");

        return new StockPrice(stockCode, price);
    }
}
