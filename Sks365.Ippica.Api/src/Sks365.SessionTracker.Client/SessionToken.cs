namespace Sks365.SessionTracker.Client
{
    public class SessionToken
    {
        private string _userName;

        public int BookmakerId { get; private set; }
        public string AspNetSession { get; private set; }
        public string Username
        {
            get { return _userName; }
            private set { _userName = value.ToLower(); }
        }

        public SessionToken(int bookmakerId, string username, string aspNetSession)
        {
            BookmakerId = bookmakerId;
            Username = username;
            AspNetSession = aspNetSession;
        }
    }
}
