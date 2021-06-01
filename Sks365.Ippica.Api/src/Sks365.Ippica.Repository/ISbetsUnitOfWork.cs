using Sks365Ippica.Repository.Abstraction;
using Sks365Ippica.Repository.Repositories;
using System;
using System.Data;

namespace Sks365Ippica.Repository
{
    public class IsbetsUnitOfWork : IIsbetsUnitOfWork
    {
        private bool _disposed;
        private readonly IDataContext _dataContext;

        public IUserRepository UserRepository { get; }
        public IWalletRepository WalletRepository { get; }
        public ICommonRepository CommonRepository { get; }

        public IsbetsUnitOfWork(IDataContext dataContext)
        {
            _dataContext = dataContext;
            UserRepository = new UserRepository(dataContext);
            WalletRepository = new WalletRepository(dataContext);
            CommonRepository = new CommonRepository(dataContext);
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
