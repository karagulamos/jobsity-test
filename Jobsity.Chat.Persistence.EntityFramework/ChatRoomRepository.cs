using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Jobsity.Chat.Core.Persistence;

public class ChatRoomRepository : IChatRoomRepository
{
    private readonly ChatContext _context;

    public ChatRoomRepository(ChatContext chatContext) => _context = chatContext;

    public Task<ChatRoom?> GetChatRoomAsync(Guid roomId)
    => _context.ChatRooms.SingleOrDefaultAsync(chatRoom => chatRoom.Id == roomId);

    public Task<ChatRoom[]> GetChatRoomsAsync() => _context.ChatRooms.ToArrayAsync();
}