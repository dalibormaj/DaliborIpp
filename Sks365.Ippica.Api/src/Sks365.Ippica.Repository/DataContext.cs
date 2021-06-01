using Dapper;
using Sks365Ippica.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sks365Ippica.Repository
{
    public class DataContext : IDataContext
    {
        #region Fields
        //private readonly IProvider _provider;
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;
        #endregion

        #region Ctor

        public DataContext(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Get parameter name and value from item collections to dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        protected virtual IDictionary<string, object> GetParameters<T>(IEnumerable<T> items)
        {
            //Check.IsNullOrEmpty(items);

            var parameters = new Dictionary<string, object>();
            var entityArray = items.ToArray();
            var entityType = entityArray[0].GetType();
            for (int i = 0; i < entityArray.Length; i++)
            {
                var properties = entityArray[i].GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var property in properties)
                    parameters.Add(property.Name + (i + 1), entityType.GetProperty(property.Name).GetValue(entityArray[i], null));
            }

            return parameters;
        }
        #endregion

        #region Sync Methods

        /// <summary>
        /// Execute with stored procedure by name
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        public virtual int ExecuteProcedure(string storedProcedureName, object parameters = null)
        {
            //Check.IsNullOrEmpty(storedProcedureName);

            //execute
            return _connection.Execute(sql: storedProcedureName,
                param: parameters,
                transaction: _transaction,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Execute reader with stored procedure by name
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        //public virtual IDataReader ExecuteReaderProcedure(string storedProcedureName, object parameters = null)
        //{
        //    //execute reader
        //    return _connection.ExecuteReader(sql: storedProcedureName,
        //        param: parameters,
        //        transaction: _transaction,
        //        commandType: CommandType.StoredProcedure);
        //}

        public virtual ICollection<IDictionary<string, object>> ExecuteReaderProcedure(string storedProcedureName, object parameters = null)
        {
            try
            {
                var res = (IEnumerable<IDictionary<string, object>>)_connection.Query(storedProcedureName,
                    param: parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure);
                return res.ToList();
            }
            catch
            {
                throw;
            }
        }

        public virtual ICollection<T> ExecuteReaderProcedure<T>(string storedProcedureName, Func<IDictionary<string, object>, T> mapper, object parameters = null)
        {
            try
            {
                var res = (IEnumerable<IDictionary<string, object>>)_connection.Query(storedProcedureName,
                    param: parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure);
                return res.Select(mapper)?.ToList();
            }
            catch
            {
                throw;
            }
        }

        public virtual ICollection<T> ExecuteReaderProcedure<T>(string storedProcedureName, object parameters = null)
        {
            try
            {
                return _connection.Query<T>(storedProcedureName,
                    param: parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure).ToList();
            }
            catch
            {
                throw;
            }
        }

        public virtual (ICollection<T>, ICollection<U>) ExecuteReaderProcedure<T, U>(string storedProcedureName, object parameters = null)
        {
            var dataSets = _connection.QueryMultiple(storedProcedureName,
                    param: parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure);

            var firstDataSet = dataSets.Read<T>().ToList();
            var secondDataSet = dataSets.Read<U>().ToList();
            return (firstDataSet, secondDataSet);
        }

        #endregion

        #region Async Methods
        public async Task<int> ExecuteProcedureAsync(string storedProcedureName, object parameters = null)
        {
            //execute
            return await _connection.ExecuteAsync(sql: storedProcedureName,
                param: parameters,
                transaction: _transaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IDataReader> ExecuteReaderProcedureAsync(string storedProcedureName, object parameters = null)
        {
            //execute reader
            var res = await _connection.ExecuteReaderAsync(sql: storedProcedureName,
                param: parameters,
                transaction: _transaction,
                commandType: CommandType.StoredProcedure);

            return res;
        }
        #endregion

        #region Context Management
        /// <summary>
        /// Begin transcation scope
        /// </summary>
        /// <returns></returns>
        public virtual IDbTransaction BeginTransaction()
        {
            OpenConnection();
            if (_transaction != null) throw new Exception("Transaction already opened! Please COMMIT/ROLLBACK transaction before opening a new one!");
            _transaction = _connection.BeginTransaction();
            return _transaction;
        }

        public virtual void Commit()
        {
            if (_transaction != null)
            {
                try
                {
                    _transaction.Commit();
                }
                catch
                {
                    _transaction.Rollback();
                    throw;
                }
                finally
                {
                    _transaction.Dispose();
                }
            }
        }

        public virtual void Rollback()
        {
            if (_transaction != null)
            {
                try
                {
                    _transaction.Rollback();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _transaction.Dispose();
                }
            }
        }

        /// <summary>
        /// Open connection with whether open or close
        /// </summary>
        public virtual void OpenConnection()
        {
            if (_connection != null &&
                _connection.State != ConnectionState.Open &&
                _connection.State != ConnectionState.Connecting)
                _connection.Open();
        }

        /// <summary>
        /// Gets the current connection
        /// </summary>
        public virtual IDbConnection Connection
        {
            get
            {
                OpenConnection();

                return _connection;
            }
        }

        /// <summary>
        /// Dispose the current connection
        /// </summary>
        public virtual void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
        }


        #endregion
    }
}
