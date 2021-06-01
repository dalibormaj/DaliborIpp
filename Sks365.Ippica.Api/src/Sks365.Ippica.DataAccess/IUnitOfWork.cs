using System;
using System.Data;

namespace Sks365.Ippica.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        IDbTransaction BeginTransaction();
        void Commit();
        void Rollback();
    }
}
