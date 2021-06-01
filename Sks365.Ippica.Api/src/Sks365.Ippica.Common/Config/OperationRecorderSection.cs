namespace Sks365.Ippica.Common.Config
{
    public class OperationRecorderSection
    {
        public string ClientName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public int ConnectTimeout { get; set; }
        public int SyncTimeout { get; set; }
        public bool AllowAdmin { get; set; }
        public int KeepAlive { get; set; }
        public bool AbortOnConnectFail { get; set; }
        public int DatabaseId { get; set; }
    }
}
