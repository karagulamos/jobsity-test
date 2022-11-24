namespace Jobsity.Chat.Core.Persistence;

public interface ICache
{
    Task<T?> GetAsync<T>(string key);

    Task SetAsync<T>(string key, T value, TimeSpan? expiry = default);
}