using Sks365Ippica.Repository.Abstraction;
using Sks365Ippica.Repository.Repositories;
using System;
using System.Data;

namespace Sks365Ippica.Repository
{
    public class MstUnitOfWork : IMstUnitOfWork
    {
        private bool _disposed;
        private readonly IDataContext _dataContext;

        public IBetRepository BetRepository { get; }

        public MstUnitOfWork(IDataContext dataContext)
        {
            _dataContext = dataContext;
            BetRepository = new BetRepository(dataContext);
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
