using System.Text.Json;
using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Core.Services;

namespace Jobsity.Chat.Services;

public class StockTickerService : IStockTickerService
{
    private readonly HttpClient _client;

    public StockTickerService(HttpClient client) => _client = client;

    public async Task<StockPrice?> GetStockPriceAsync(string stockCode)
    {
        var response = await _client.GetAsync($"?s={stockCode}s&f=sd2t2ohlcv&h&e=csv");

        if (!response.IsSuccessStatusCode)
            throw new Exception("Error getting stock price");

        var content = await response.Content.ReadAsStringAsync();
        var stockPrice = JsonSerializer.Deserialize<StockPrice>(content);

        return stockPrice;
    }
}
