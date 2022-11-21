using Microsoft.AspNetCore.Mvc;

namespace Jobsity.Chat.Web.Controllers;

[Route("/api/stocks")]
[ApiController]
public class StockTickerController : Controller
{
    [HttpPost]
    public Task Post()
    {
        return Task.CompletedTask;
    }
}