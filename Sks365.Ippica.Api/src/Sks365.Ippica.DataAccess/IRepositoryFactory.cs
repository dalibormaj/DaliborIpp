using Sks365.Ippica.DataAccess.Repositories.Abstraction;

namespace Sks365.Ippica.DataAccess
{
    public interface IRepositoryFactory
    {
        IDataContext DataContext { get; }
        T Create<T>() where T : class, IRepository;
    }
}