using System;

namespace Sks365.SessionTracker.Client
{
    public class SessionData
    {
        public bool SessionExists { get; private set; }
        public TimeSpan TimeToLive { get; private set; }

        public string Username { get; private set; }
        public int BookmakerId { get; private set; }
        public int? ApplicationTypeId { get; private set; }

        public SessionData(bool exist, string userName, int bookmakerId, TimeSpan timeToLive, int? applicationTypeId = null)
        {
            SessionExists = exist;
            Username = userName;
            BookmakerId = bookmakerId;
            TimeToLive = timeToLive;
            ApplicationTypeId = applicationTypeId;
        }

        public static SessionData SessionNotExist()
        {
            return new SessionData(false, string.Empty, 1, TimeSpan.Zero);
        }
    }
}
