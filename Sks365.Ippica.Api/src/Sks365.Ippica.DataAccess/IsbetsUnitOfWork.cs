using Sks365.Ippica.DataAccess.Repositories;
using Sks365.Ippica.DataAccess.Repositories.Abstraction;
using System;
using System.Data;

namespace Sks365.Ippica.DataAccess
{
    public class IsbetsUnitOfWork : IIsbetsUnitOfWork
    {
        private bool _disposed;
        private readonly IDataContext _dataContext;

        public virtual IUserRepository UserRepository { get; }
        public virtual IWalletRepository WalletRepository { get; }
        public virtual ICommonRepository CommonRepository { get; }
        public virtual IPaymentOrderRepository PaymentOrderRepository { get; }

        public IsbetsUnitOfWork(IRepositoryFactory factory)
        {
            _dataContext = factory.DataContext;
            UserRepository = factory.Create<UserRepository>();
            WalletRepository = factory.Create<WalletRepository>();
            CommonRepository = factory.Create<CommonRepository>();
            PaymentOrderRepository = factory.Create<PaymentOrderRepository>();
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
