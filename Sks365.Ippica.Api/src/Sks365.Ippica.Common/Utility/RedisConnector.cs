using Sks365.Ippica.Common.Exceptions;
using StackExchange.Redis;
using System;

namespace Sks365.Ippica.Common.Utility
{
    public class RedisConnector : IRedisConnector
    {
        private Lazy<ConnectionMultiplexer> lazyConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisConnector" /> class.
        /// </summary>
        /// <param name="connectionStrings">The connection strings.</param>
        public RedisConnector(ConfigurationOptions options)
        {
            if ((options.EndPoints?.Count ?? 0) == 0)
            {
                throw new IppicaException(ReturnCodeEnum.Unknown, "Redis host is missing");
            }

            if (string.IsNullOrEmpty(options.ClientName))
            {
                throw new IppicaException(ReturnCodeEnum.Unknown, "Redis ClientName is missing");
            }

            lazyConnection = SetLazyConnection(options);
        }

        public ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        private Lazy<ConnectionMultiplexer> SetLazyConnection(ConfigurationOptions options)
        {
            return new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(options);
            });
        }

        public void Dispose()
        {
            lazyConnection.Value.Dispose();
        }
    }
}