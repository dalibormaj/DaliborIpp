using Sks365.Ippica.DataAccess.Repositories.Abstraction;

namespace Sks365.Ippica.DataAccess
{
    public interface IMstUnitOfWork : IUnitOfWork
    {
        IBetRepository BetRepository { get; }
    }
}
