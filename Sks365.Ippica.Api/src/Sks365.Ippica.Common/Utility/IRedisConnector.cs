using StackExchange.Redis;

namespace Sks365.Ippica.Common.Utility
{
    public interface IRedisConnector
    {
        ConnectionMultiplexer Connection { get; }
    }
}
