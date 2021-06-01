using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model.Enums;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Sks365.Ippica.Application.Utility.OperationRecorder
{
    /// <summary>
    /// Operation recorder can be used to detect if some other operation for the same ticketId/ExternalId is 
    /// still running. The goal is to avoid conflicts among different requests so that just one request per 
    /// ticketId/ExternalId can be processed at the same time.
    /// </summary>
    public class OperationRecorderExecutor : IOperationRecorderExecutor
    {
        private string _tickerId;
        private string _externalId;
        private readonly IRedisConnector _redisConnector;
        private readonly int _databaseId;
        private readonly string _key;
        private readonly BetRequestTypeEnum _operationType;
        private bool _isRecordingStarted = false;
        private const string KEY_PREFIX = "Sks365.Ippica.Api:OperationsRunning";
        private const int DEFAULT_EXPIRATION = 120;

        public OperationRecorderExecutor(IRedisConnector redisConnector, int databaseId, string ticketId, string externalId, BetRequestTypeEnum operationType)
        {
            _tickerId = ticketId;
            _externalId = externalId;
            _redisConnector = redisConnector;
            _databaseId = databaseId;
            _operationType = operationType;

            var id = !string.IsNullOrEmpty(_externalId) ? _externalId : _tickerId;
            if (string.IsNullOrEmpty(id)) throw new IppicaException(ReturnCodeEnum.Unknown, "TicketId/ExternaId is missing");

            _key = $"{KEY_PREFIX}:{id}";
        }

        public async void Dispose()
        {
            await OperationStop();
        }

        public async Task<bool> IsAnyOperationRunning()
        {
            var redisDatabase = _redisConnector.Connection.GetDatabase(_databaseId) as IDatabaseAsync;
            var keyExists = await redisDatabase.KeyExistsAsync(_key);
            return keyExists;
        }

        /// <summary>
        /// Is there any other operation still running despite selected typeId
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public async Task<bool> IsAnyOtherOperationRunningBut(BetRequestTypeEnum typeId)
        {
            var operationExists = false;
            var redisDatabase = _redisConnector.Connection.GetDatabase(_databaseId) as IDatabaseAsync;

            foreach (BetRequestTypeEnum type in Enum.GetValues(typeof(BetRequestTypeEnum)))
            {
                if (type != typeId)
                {
                    operationExists = await redisDatabase.SetContainsAsync(_key, type.ToString());
                    break;
                }
            }

            return operationExists;
        }

        public async Task<bool> IsOperationRunning(BetRequestTypeEnum typeId)
        {
            var redisDatabase = _redisConnector.Connection.GetDatabase(_databaseId) as IDatabaseAsync;
            var operationExists = await redisDatabase.SetContainsAsync(_key, typeId.ToString());

            return operationExists;
        }
        /// <summary>
        /// Method marks that the operation has started by adding a value in Redis DB. The value 
        /// will be used to block other requests while the action (BetRequestType) is in progress. 
        /// </summary>
        /// <param name="throwIfParallelOperationRunning"></param>
        /// <returns></returns>
        public async Task OperationStart(bool throwIfParallelOperationRunning = true)
        {
            if (throwIfParallelOperationRunning && await IsAnyOperationRunning())
                throw new IppicaException(ReturnCodeEnum.AnotherBetOperationStillRunning);

            var redisDatabase = _redisConnector.Connection.GetDatabase(_databaseId) as IDatabaseAsync;

            await redisDatabase.SetAddAsync(_key, _operationType.ToString());
            await redisDatabase.KeyExpireAsync(_key, TimeSpan.FromSeconds(DEFAULT_EXPIRATION));

            _isRecordingStarted = true;
        }

        public async Task OperationStop()
        {
            if (_isRecordingStarted) //delete only data insterted by this process
            {
                var redisDatabase = _redisConnector.Connection.GetDatabase(_databaseId) as IDatabaseAsync;
                await redisDatabase.SetRemoveAsync(_key, _operationType.ToString());
                _isRecordingStarted = false;
            }
        }
    }
}
