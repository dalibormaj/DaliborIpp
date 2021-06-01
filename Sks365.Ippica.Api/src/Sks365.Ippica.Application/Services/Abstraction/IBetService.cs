using Sks365.Ippica.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sks365.Ippica.Application.Services.Abstraction
{
    public interface IBetService
    {
        Task<Bet> Reserve(BetRequest betRequest, Bet bet, List<BetTransaction> betTransactions);
        Task<Bet> Place(BetRequest betRequest, Bet bet, List<BetTransaction> betTransactions);
        Task<Bet> SettleWin(BetRequest betRequest, List<BetTransaction> betTransactions, bool processTransactionsAsPending = false);
        Task<Bet> SettleLoss(BetRequest betRequest);
        Task<Bet> Reopen(BetRequest betRequest);
        Task<Bet> CancelStake(BetRequest betRequest, List<BetTransaction> betTransactions = null, bool processTransactionsAsPending = false);
        Task<Bet> UndoCancelStake(BetRequest betRequest, List<BetTransaction> betTransactions = null);
        Task<Bet> CancelWin(BetRequest betRequest, List<BetTransaction> betTransactions = null);
    }
}