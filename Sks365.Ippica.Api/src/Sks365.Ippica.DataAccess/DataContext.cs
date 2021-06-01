using AutoMapper;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sks365.Ippica.DataAccess
{
    public class DataContext : IDataContext
    {
        #region Fields

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
            var res = _connection.Execute(sql: storedProcedureName,
                param: parameters,
                transaction: _transaction,
                commandType: CommandType.StoredProcedure,
                commandTimeout: _connection.ConnectionTimeout);

            return res;
        }


        public virtual List<IDictionary<string, object>> ExecuteReaderProcedure(string storedProcedureName, object parameters = null)
        {
            var res = (IEnumerable<IDictionary<string, object>>)_connection.Query(storedProcedureName,
                param: parameters,
                transaction: _transaction,
                commandType: CommandType.StoredProcedure,
                commandTimeout: _connection.ConnectionTimeout);

            return res.ToList();
        }

        public virtual List<T> ExecuteReaderProcedure<T>(string storedProcedureName, IMapper mapper, object parameters = null)
        {
            var res = (IEnumerable<IDictionary<string, object>>)_connection.Query(storedProcedureName,
               param: parameters,
               transaction: _transaction,
               commandType: CommandType.StoredProcedure,
               commandTimeout: _connection.ConnectionTimeout);

            return mapper.Map<List<T>>(res);
        }

        public virtual List<T> ExecuteReaderProcedure<T>(string storedProcedureName, object parameters = null)
        {
            var res = _connection.Query<T>(storedProcedureName,
                param: parameters,
                transaction: _transaction,
                commandType: CommandType.StoredProcedure,
                commandTimeout: _connection.ConnectionTimeout).ToList();

            return res;
        }

        public virtual (List<T>, List<U>) ExecuteReaderProcedure<T, U>(string storedProcedureName, object parameters = null)
        {
            var dataSets = _connection.QueryMultiple(storedProcedureName,
                    param: parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _connection.ConnectionTimeout);

            var firstDataSet = dataSets.Read<T>().ToList();
            var secondDataSet = dataSets.Read<U>().ToList();

            return (firstDataSet, secondDataSet);
        }

        #endregion

        #region Async Methods

        public async Task<List<IDictionary<string, object>>> ExecuteReaderProcedureAsync(string storedProcedureName, object parameters = null)
        {
            var res = await _connection.QueryAsync(storedProcedureName,
                param: parameters,
                transaction: _transaction,
                commandType: CommandType.StoredProcedure,
                commandTimeout: _connection.ConnectionTimeout);

            //has to be done like this because Dapper for some reason does not retrieve the object 
            //in the same way in case of QueryAsync and Query. Due to that, the cast cannot be done in an easier way
            //and has to be done element by element
            var resConverted = res.Select(x => (IDictionary<string, object>)x).ToList();

            return resConverted;
        }

        public async Task<List<T>> ExecuteReaderProcedureAsync<T>(string storedProcedureName, object parameters = null)
        {
            var res = await _connection.QueryAsync<T>(storedProcedureName,
                    param: parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _connection.ConnectionTimeout);

            return res.ToList();
        }

        public async Task<List<T>> ExecuteReaderProcedureAsync<T>(string storedProcedureName, IMapper mapper, object parameters = null)
        {
            var res = await _connection.QueryAsync(storedProcedureName,
                param: parameters,
                transaction: _transaction,
                commandType: CommandType.StoredProcedure,
                commandTimeout: _connection.ConnectionTimeout);

            return mapper.Map<List<T>>(res);
        }

        public async Task<int> ExecuteProcedureAsync(string storedProcedureName, object parameters = null)
        {
            var res = await _connection.ExecuteAsync(sql: storedProcedureName,
                param: parameters,
                transaction: _transaction,
                commandType: CommandType.StoredProcedure,
                commandTimeout: _connection.ConnectionTimeout);

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
                    _transaction = null;
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
                    _transaction = null;
                }
            }
        }

        /// <summary>
        /// Open connection
        /// </summary>
        public virtual void OpenConnection()
        {
            if (_connection != null &&
                _connection.State != ConnectionState.Open &&
                _connection.State != ConnectionState.Connecting)
                _connection.Open();
        }

        /// <summary>
        /// Close connection 
        /// </summary>
        public virtual void CloseConnection()
        {
            if (_connection != null &&
                _connection.State != ConnectionState.Closed)
                _connection.Close();
        }

        /// <summary>
        /// Gets the current connection
        /// </summary>
        public virtual IDbConnection Connection
        {
            get
            {
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
