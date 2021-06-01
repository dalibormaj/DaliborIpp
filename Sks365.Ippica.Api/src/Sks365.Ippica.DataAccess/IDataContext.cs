using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Sks365.Ippica.DataAccess
{
    public interface IDataContext : IDisposable
    {
        #region Sync Methods
        int ExecuteProcedure(string storedProcedureName, object parameters = null);
        List<IDictionary<string, object>> ExecuteReaderProcedure(string storedProcedureName, object parameters = null);
        List<T> ExecuteReaderProcedure<T>(string storedProcedureName, object parameters = null);
        List<T> ExecuteReaderProcedure<T>(string storedProcedureName, IMapper mapper, object parameters = null);
        (List<T>, List<U>) ExecuteReaderProcedure<T, U>(string storedProcedureName, object parameters = null);
        #endregion

        #region Async Methods
        Task<int> ExecuteProcedureAsync(string storedProcedureName, object parameters = null);
        Task<List<IDictionary<string, object>>> ExecuteReaderProcedureAsync(string storedProcedureName, object parameters = null);
        Task<List<T>> ExecuteReaderProcedureAsync<T>(string storedProcedureName, object parameters = null);
        Task<List<T>> ExecuteReaderProcedureAsync<T>(string storedProcedureName, IMapper mapper, object parameters = null);
        #endregion

        IDbConnection Connection { get; }
        void OpenConnection();
        IDbTransaction BeginTransaction();
        void Commit();
        void Rollback();
    }
}
