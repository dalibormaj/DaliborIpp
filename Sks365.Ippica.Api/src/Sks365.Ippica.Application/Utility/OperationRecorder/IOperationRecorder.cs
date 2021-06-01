using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.Application.Utility.OperationRecorder
{
    public interface IOperationRecorder
    {
        IOperationRecorderExecutor CreateExecutor(string ticketId, string externalId, BetRequestTypeEnum operationType);
    }
}
