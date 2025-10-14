using StackExchange.Redis;

namespace WorldMap.Infrastructure
{
    public interface IRedisConnection
    {
        IDatabase GetDatabase(int dbNumber = -1, object asyncState = null);
    }
}