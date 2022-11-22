using Jobsity.Chat.Core.Models;

namespace Jobsity.Chat.Core.Persistence;

public interface IChatRepository
{
    /// <summary>
    /// Gets the latest chats ordered by date created. 
    /// The number of chats to retrieve is specified by the <paramref name="count"/> parameter.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<UserChat[]> GetLatestAsync(Guid roomId, int count);

    /// <summary>
    /// Adds a new chat to the repository.
    /// </summary>
    Task AddAsync(UserChat chat);
}