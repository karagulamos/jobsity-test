using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jobsity.Chat.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    public string UserId { get; set; } = string.Empty;

    public IActionResult OnPost()
    {
        if (ModelState.IsValid)
        {
            return RedirectToPage("Chat", new { userId = UserId });
        }

        return Page();
    }
}
