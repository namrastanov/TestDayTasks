using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldMap.Infrastructure
{
    public class RedisConnection : IRedisConnection
    {
        private readonly IConnectionMultiplexer _connection;
        public RedisConnection(IConnectionMultiplexer connection)
        {
            _connection = connection;
        }
        public IDatabase GetDatabase(int dbNumber = -1, object asyncState = null)
        {
            return _connection.GetDatabase(dbNumber, asyncState);
        }
    }
}
