using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.Application.Utility.OperationRecorder
{
    /// <summary>
    /// Operation recorder can be used to detect if some other operation for the same ticketId/ExternalId is 
    /// still running. The goal is to avoid conflicts among different requests so that just one request per 
    /// ticketId/ExternalId can be processed at the same time.
    /// </summary>
    public class OperationRecorder : IOperationRecorder
    {
        private readonly IRedisConnector _redisConnector;
        private readonly int _databaseId;

        public IOperationRecorderExecutor CreateExecutor(string ticketId, string externalId, BetRequestTypeEnum operationType)
                                          => new OperationRecorderExecutor(_redisConnector, _databaseId, ticketId, externalId, operationType);

        public OperationRecorder(IRedisConnector redisConnector, int databaseId)
        {
            _redisConnector = redisConnector;
            _databaseId = databaseId;
        }
    }
}
