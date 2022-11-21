using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Core.Persistence;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Jobsity.Chat.Web.Controllers
{
    [Route("api/chats")]
    public class ChatController : Controller
    {
        private readonly IChatRepository _chats;

        public ChatController(IChatRepository chats)
        {
            _chats = chats;
        }

        // GET: api/chats
        [HttpGet]
        public Task<UserChat[]> Get()
        {
            return _chats.GetLatestAsync();
        }
    }
}

