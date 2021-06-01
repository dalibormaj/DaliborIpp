using Sks365.Ippica.DataAccess.Repositories.Abstraction;
using System;

namespace Sks365.Ippica.DataAccess
{
    public class RepositoryFactory : IRepositoryFactory
    {
        public IDataContext DataContext { get; private set; }

        public RepositoryFactory(IDataContext dataContext)
        {
            DataContext = dataContext;
        }
        public T Create<T>() where T : class, IRepository
        {
            return (T)Activator.CreateInstance(typeof(T), DataContext);
        }
    }
}
