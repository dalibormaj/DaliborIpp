using Sks365.Ippica.DataAccess.Repositories;
using Sks365.Ippica.DataAccess.Repositories.Abstraction;
using System;
using System.Data;

namespace Sks365.Ippica.DataAccess
{
    public class MstUnitOfWork : IMstUnitOfWork
    {
        private bool _disposed;
        private readonly IDataContext _dataContext;

        public virtual IBetRepository BetRepository { get; }

        public MstUnitOfWork(IRepositoryFactory factory)
        {
            _dataContext = factory.DataContext;
            BetRepository = factory.Create<BetRepository>();
        }

        public IDbTransaction BeginTransaction()
        {
            return _dataContext.BeginTransaction();
        }

        public void Commit()
        {
            _dataContext.Commit();
        }

        public void Rollback()
        {
            _dataContext.Rollback();
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_dataContext != null)
                    {
                        _dataContext.Dispose();
                    }
                }
                _disposed = true;
            }
        }

    }
}
