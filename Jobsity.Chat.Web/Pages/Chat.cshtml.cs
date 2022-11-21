using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Core.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jobsity.Chat.Web.Pages
{
	public class ChatModel : PageModel
    {
        private readonly ILogger<ChatModel> _logger;
        private readonly IChatRepository _chats;

        public ChatModel(ILogger<ChatModel> logger, IChatRepository chats)
        {
            _logger = logger;
            _chats = chats;

            Chats = Array.Empty<UserChat>();
        }

        public IList<UserChat> Chats { get; set; }

        public async Task OnGetAsync()
        {
            var latestChats = await _chats.GetLatestAsync();
            Chats = latestChats.OrderBy(c => c.DateCreated).ToList();
        }
    }
}
