using System.Collections.Generic;

namespace Sks365.SessionTracker.Client.Configuration
{
    public class SessionTrackerSettings
    {
        public string SessionCryptoKey { get; set; }
        public string ClientName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public int ConnectTimeout { get; set; }
        public int SyncTimeout { get; set; }
        public bool AllowAdmin { get; set; }
        public int KeepAlive { get; set; }
        public bool AbortOnConnectFail { get; set; }
        public List<RedisDatabase> Databases { get; set; }
    }
}
