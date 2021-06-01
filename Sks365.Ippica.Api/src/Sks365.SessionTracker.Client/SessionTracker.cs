using Microsoft.Extensions.Logging;
using Sks365.ApplicationLog.Core;
using Sks365.Ippica.Common.Utility;
using Sks365.SessionTracker.Client.Configuration;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sks365.SessionTracker.Client
{
    public class SessionTracker : ISessionTracker
    {
        private readonly ILogger<SessionTracker> _logger;
        private readonly SessionTrackerSettings _settings;
        private readonly ConnectionMultiplexer _redisConnection;

        public SessionTracker(ILogger<SessionTracker> logger, IRedisConnector redisConnector, SessionTrackerSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(SessionTrackerSettings));

            if (string.IsNullOrEmpty(settings?.Host))
                throw new NullReferenceException("Redis host is missing");

            if (settings.Databases == null || (settings.Databases?.Count ?? 0) == 0)
                throw new ArgumentException("Redis databases are not set");

            _redisConnection = redisConnector.Connection;

            _logger = logger;
            _settings = settings;
        }

        public async Task<SessionData> GetSession(string sessionTokenEncrypted)
        {
            SessionToken sessionToken;
            try
            {
                sessionToken = CreateSessionTokenFromString(sessionTokenEncrypted);
            }
            catch (Exception ex)
            {
                _logger.Here().LogWarning(ex, "Error creating session token from encrypted string: {sessionTokenEncrypted}", sessionTokenEncrypted);
                return SessionData.SessionNotExist();
            }

            return await GetSession(sessionToken);
        }

        private SessionToken CreateSessionTokenFromString(string sessionTokenEncrypted)
        {
            if (string.IsNullOrEmpty(sessionTokenEncrypted))
                throw new ArgumentException("Session string is null or empty.");

            string sessionTokenDecrypted;

            try
            {
                sessionTokenDecrypted = CryptographyTool.DecryptMD5(sessionTokenEncrypted, _settings.SessionCryptoKey, true, false);
            }
            catch (Exception ex)
            {
                throw new Exception("CryptographyTool can not decrypt session.", ex);
            }

            string[] tokenParts = sessionTokenDecrypted.Split('&');

            if (tokenParts.Length != 3)
            {
                throw new Exception("Token parts must have 3 parts.");
            }

            int bookmakerId = Convert.ToInt32(tokenParts[0]);
            string username = tokenParts[1];
            string aspNetSession = tokenParts[2];

            return new SessionToken(bookmakerId, username, aspNetSession);
        }


        private async Task<SessionData> GetSession(SessionToken sessionToken)
        {
            foreach (RedisDatabase redisDatabase in _settings.Databases)
            {
                var sessionData = await GetSessionFromRedis(sessionToken, redisDatabase);

                if (sessionData.SessionExists)
                    return sessionData;
            }

            return SessionData.SessionNotExist();
        }

        private async Task<SessionData> GetSessionFromRedis(SessionToken sessionToken, RedisDatabase redisDatabase)
        {
            var database = _redisConnection.GetDatabase(redisDatabase.ID) as IDatabaseAsync;
            if (database == null)
            {
                throw new NullReferenceException($"Redis database: { redisDatabase.ID } not exist!");
            }

            string key = $"Sks365:SessionTracker:UserSessions:{sessionToken.BookmakerId}-{sessionToken.Username}";

            var keyExists = await database.KeyExistsAsync(key);
            if (!keyExists)
            {
                key = $"Sks365:SessionTracker:UserSessions:{sessionToken.BookmakerId}-{sessionToken.Username}-{sessionToken.AspNetSession}";
                keyExists = await database.KeyExistsAsync(key);
            }

            if (!keyExists)
            {
                return SessionData.SessionNotExist();
            }
            else if (await database.KeyTypeAsync(key) == RedisType.Set)
            {
                var sessionExists = await database.SetContainsAsync(key, sessionToken.AspNetSession);
                var timeToLive = await database.KeyTimeToLiveAsync(key);
                timeToLive = timeToLive ?? TimeSpan.MaxValue;

                return new SessionData(sessionExists, sessionToken.Username, sessionToken.BookmakerId, timeToLive.Value);
            }
            else if (await database.KeyTypeAsync(key) == RedisType.String)
            {
                var sessionExists = true;
                var timeToLive = await database.KeyTimeToLiveAsync(key);
                timeToLive = timeToLive ?? TimeSpan.MaxValue;

                return new SessionData(sessionExists, sessionToken.Username, sessionToken.BookmakerId, timeToLive.Value);
            }
            else if (await database.KeyTypeAsync(key) == RedisType.Hash)
            {
                var list = (await database.HashGetAllAsync(key)).ToList();
                int? applicationTypeId = (int?)list.Find(x => x.Name == "IDTipoApplicazione").Value;

                var sessionExists = true;
                var timeToLive = await database.KeyTimeToLiveAsync(key);
                timeToLive = timeToLive ?? TimeSpan.MaxValue;

                return new SessionData(sessionExists, sessionToken.Username, sessionToken.BookmakerId, timeToLive.Value, applicationTypeId);
            }
            else if (await database.KeyTypeAsync(key) == RedisType.List)
            {
                var listLength = await database.ListLengthAsync(key);
                RedisValue session;
                for (int i = 0; i < listLength; i++)
                {
                    session = await database.ListGetByIndexAsync(key, i);
                    if (session.HasValue && session == sessionToken.AspNetSession)
                    {
                        var timeToLive = (await database.KeyTimeToLiveAsync(key)).Value;


                        return new SessionData(true, sessionToken.Username, sessionToken.BookmakerId, timeToLive);
                    }
                }
            }

            return SessionData.SessionNotExist();
        }
    }
}