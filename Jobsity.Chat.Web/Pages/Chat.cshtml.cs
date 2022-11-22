using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Core.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jobsity.Chat.Web.Pages
{
    public class ChatModel : PageModel
    {
        private const int DefaultChatCount = 50;

        private readonly ILogger<ChatModel> _logger;
        private readonly IChatRepository _chats;
        private readonly IChatRoomRepository _chatRooms;

        public ChatModel(ILogger<ChatModel> logger, IChatRepository chats, IChatRoomRepository chatRooms)
        {
            _logger = logger;
            _chats = chats;
            _chatRooms = chatRooms;

            Chats = Array.Empty<UserChat>();
        }

        public IList<UserChat> Chats { get; set; }

        [BindProperty(SupportsGet = true, Name = "roomId")]
        public Guid RoomId { get; set; }

        [BindProperty(SupportsGet = true, Name = "userId")]
        public string UserId { get; set; } = string.Empty;

        public string ChatRoomName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGet()
        {
            try
            {
                var chatRoom = await _chatRooms.GetChatRoomAsync(RoomId);
                if (chatRoom == null)
                {
                    _logger.LogWarning($"Chat room {RoomId} not found");
                    return NotFound();
                }

                ChatRoomName = chatRoom.Name;

                var latestChats = await _chats.GetLatestAsync(RoomId, DefaultChatCount);
                Chats = latestChats.OrderBy(c => c.DateCreated).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest chats");
                return RedirectToPage("Error");
            }

            return Page();
        }
    }
}
