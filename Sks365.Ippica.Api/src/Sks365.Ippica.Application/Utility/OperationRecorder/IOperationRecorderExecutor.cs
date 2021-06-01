using Sks365.Ippica.Domain.Model.Enums;
using System;
using System.Threading.Tasks;

namespace Sks365.Ippica.Application.Utility.OperationRecorder
{
    public interface IOperationRecorderExecutor : IDisposable
    {
        Task<bool> IsAnyOperationRunning();
        Task<bool> IsAnyOtherOperationRunningBut(BetRequestTypeEnum typeId);
        Task<bool> IsOperationRunning(BetRequestTypeEnum typeId);
        Task OperationStart(bool throwIfParallelOperationRunning = true);
        Task OperationStop();
    }
}
