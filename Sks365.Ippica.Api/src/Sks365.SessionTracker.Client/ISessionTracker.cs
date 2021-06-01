using System.Threading.Tasks;

namespace Sks365.SessionTracker.Client
{
    public interface ISessionTracker
    {
        Task<SessionData> GetSession(string sessionTokenEncrypted);
    }
}
