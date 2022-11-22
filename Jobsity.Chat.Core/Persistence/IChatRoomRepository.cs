namespace Jobsity.Chat.Core.Persistence;

using System.Threading.Tasks;
using Jobsity.Chat.Core.Models;

public interface IChatRoomRepository
{
    Task<ChatRoom?> GetChatRoomAsync(Guid roomId);
    Task<ChatRoom[]> GetChatRoomsAsync();
}