using Jobsity.Chat.Core.Models;
using Jobsity.Chat.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Jobsity.Chat.Persistence.EntityFramework;

public class ChatRepository : IChatRepository
{
    private readonly ChatContext _context;

    public ChatRepository(ChatContext chatContext) => _context = chatContext;

    public async Task AddAsync(UserChat chat)
    {
        await _context.UserChats.AddAsync(chat);
        await _context.SaveChangesAsync();
    }

    public Task<UserChat[]> GetLatestAsync(Guid roomId, int count)
    => _context.UserChats.Where(chat => chat.RoomId == roomId)
                         .OrderByDescending(u => u.DateCreated)
                         .Take(count)
                         .OrderBy(u => u.DateCreated)
                         .ToArrayAsync();

    public Task<UserChat[]> GetSinceAsync(DateTime lastDate, Guid roomId)
    => _context.UserChats.Where(chat => chat.RoomId == roomId && chat.DateCreated > lastDate)
                         .OrderBy(u => u.DateCreated)
                         .ToArrayAsync();
}