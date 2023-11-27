using System.Net;
using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Core.Models.Options;
using Jobsity.Chat.Core.Persistence;
using Jobsity.Chat.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace Jobsity.Chat.Tests;

[TestClass]
public class StockTickerServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHandler = new();
    private readonly Mock<ICache> _mockCache = new();
    private readonly Mock<IOptions<StockTickerOptions>> _mockOptions = new();

    private readonly HttpClient _mockedHttpClient;

    public StockTickerServiceTests()
    {
        _mockedHttpClient = new HttpClient(_mockHandler.Object)
        {
            BaseAddress = new Uri("https://test.example/")
        };

        _mockOptions.SetupGet(x => x.Value).Returns(new StockTickerOptions
        {
            StockQuoteCacheDuration = TimeSpan.FromMinutes(5)
        });
    }


    [TestMethod]
    public async Task GetQuoteAsync_ShouldReturnQuote_WhenSymbolValid()
    {
        // Arrange
        var symbol = "MSFT.US";

        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "Symbol,Date,Time,Open,High,Low,Close,Volume\r\n" +
                    $"{symbol},2023-11-24,19:00:04,377.33,377.97,375.135,377.43,10176649\r\n"
                )
            });

        var sut = new StockTickerService(_mockedHttpClient, _mockCache.Object, _mockOptions.Object);

        // Act
        var result = await sut.GetQuoteAsync(symbol);

        // Assert
        Assert.AreEqual(result, new StockQuote(symbol, 377.43m));
    }

    [TestMethod]
    public async Task GetQuoteAsync_ShouldReturnCachedQuote_WhenSymbolValidAndCached()
    {
        // Arrange
        var symbol = "MSFT.US";

        _mockCache
            .Setup(x => x.GetAsync<StockQuote>(symbol))
            .ReturnsAsync(new StockQuote(symbol, 377.43m));

        var sut = new StockTickerService(_mockedHttpClient, _mockCache.Object, _mockOptions.Object);

        // Act
        var result = await sut.GetQuoteAsync(symbol);

        // Assert
        Assert.AreEqual(result, new StockQuote(symbol, 377.43m));
    }

    [TestMethod]
    public async Task GetQuoteAsync_ShouldThrowException_WhenSymbolInvalid()
    {
        // Arrange
        var symbol = "INVALID";

        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "Symbol,Date,Time,Open,High,Low,Close,Volume\r\n" +
                    $"{symbol},N/D,N/D,N/D,N/D,N/D,N/D,N/D\r\n"
                )
            });

        var sut = new StockTickerService(_mockedHttpClient, _mockCache.Object, _mockOptions.Object);

        // Act
        var result = await Assert.ThrowsExceptionAsync<Exception>(() => sut.GetQuoteAsync(symbol));

        // Assert
        Assert.AreEqual("Error parsing stock price", result.Message);
    }

    [TestMethod]
    public async Task GetQuoteAsync_ShouldThrowException_WhenSymbolNotFound()
    {
        // Arrange
        var symbol = "INVALID";

        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "Symbol,Date,Time,Open,High,Low,Close,Volume\r\n"
                )
            });

        var sut = new StockTickerService(_mockedHttpClient, _mockCache.Object, _mockOptions.Object);

        // Act
        var result = await Assert.ThrowsExceptionAsync<Exception>(() => sut.GetQuoteAsync(symbol));

        // Assert
        Assert.AreEqual("Invalid stock report format", result.Message);
    }

    [TestMethod]
    public async Task GetQuoteAsync_ShouldThrowException_WhenResponseNotSuccess()
    {
        // Arrange
        var symbol = "MSFT.US";

        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var sut = new StockTickerService(_mockedHttpClient, _mockCache.Object, _mockOptions.Object);

        // Act
        var result = await Assert.ThrowsExceptionAsync<Exception>(() => sut.GetQuoteAsync(symbol));

        // Assert
        Assert.AreEqual("Error getting stock quote: BadRequest", result.Message);
    }
}