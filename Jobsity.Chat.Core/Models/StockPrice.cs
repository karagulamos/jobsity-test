namespace Jobsity.Chat.Core.Models;

public class StockPrice
{
    public StockPrice(string stockCode, decimal price)
    {
        Code = stockCode;
        Price = price;
    }

    public string Code { get; }
    public decimal Price { get; set; }
}