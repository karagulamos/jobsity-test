namespace Jobsity.Chat.Core.Persistence;

using System.Threading.Tasks;
using Jobsity.Chat.Core.Models;

public interface IChatRoomRepository
{
    /// <summary>
    /// Retrieves a chat room by its id.
    /// </summary>
    /// <param name="roomId"></param>
    /// <returns></returns>
    Task<ChatRoom?> GetChatRoomAsync(Guid roomId);

    /// <summary>
    /// Returns all chat rooms.
    /// </summary>
    /// <returns></returns>
    Task<ChatRoom[]> GetChatRoomsAsync();
}