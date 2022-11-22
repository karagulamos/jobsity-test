using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Core.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Jobsity.Chat.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IChatRoomRepository _chatRooms;

    public IndexModel(ILogger<IndexModel> logger, IChatRoomRepository chatRooms)
    {
        _logger = logger;
        _chatRooms = chatRooms;
    }

    [BindProperty]
    public string UserId { get; set; } = string.Empty;

    [BindProperty]
    public Guid RoomId { get; set; }

    public SelectList ChatRooms { get; set; } = new SelectList(Array.Empty<ChatRoom>());

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var chatRooms = await _chatRooms.GetChatRoomsAsync();
            _logger.LogInformation($"Found {chatRooms.Length} chat rooms");

            ChatRooms = new SelectList(chatRooms, nameof(ChatRoom.Id), nameof(ChatRoom.Name));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting chat rooms");
            ModelState.AddModelError(nameof(ChatRooms), "Error getting chat rooms");
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        if (ModelState.IsValid)
        {
            _logger.LogInformation($"Redirecting to chat room {RoomId} with user {UserId}");
            return Redirect($"/chat/{RoomId}/{UserId}");
        }

        return Page();
    }
}
