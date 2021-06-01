using Sks365.Ippica.Application.Utility;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.Ippica.Domain.Utility;
using Sks365.Ippica.FakeData.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sks365.Ippica.Application.UnitTests.Helpers
{
    public enum BetHelperAction
    {
        None,
        Create_Fix_Reserved,
        Create_Fix_Placed,
        Create_Fix_Refunded,
        Create_Fix_RefundedNotPaid,
        Create_Fix_Won,
        Create_Fix_WonNotPaid,
        Create_Fix_Lost,
        Create_Fix_UndoRefund,

        Create_Psr_Reserved,
        Create_Psr_Placed,
        Create_Psr_Refunded,
        Create_Psr_RefundedNotPaid,
        Create_Psr_Won,
        Create_Psr_WonNotPaid,
        Create_Psr_Lost,
        Create_Psr_Refunded_ByStakeCompensation,
        Create_Psr_RefundedNotPaid_ByStakeCompensation,
        Create_Psr_Won_IncludingStakeCompensation,
        Create_Psr_WonNotPaid_IncludingStakeCompensation,
        Create_Psr_UndoRefund,

        Create_Psip_Reserved,
        Create_Psip_Placed,
        Create_Psip_Refunded,
        Create_Psip_RefundedNotPaid,
        Create_Psip_Won,
        Create_Psip_WonNotPaid,
        Create_Psip_Lost,
        Create_Psip_Refunded_ByStakeCompensation,
        Create_Psip_RefundedNotPaid_ByStakeCompensation,
        Create_Psip_Won_IncludingStakeCompensation,
        Create_Psip_WonNotPaid_IncludingStakeCompensation,
        Create_Psip_UndoRefund
    }

    public class BetHelper
    {
        private Bet _fakeBet;
        //private BetRequest _fakeBetRequest;
        private List<BetTransaction> _fakeBetTransactions = new List<BetTransaction>();

        public BetHelper(BetHelperAction action)
        {
            if (action != BetHelperAction.None)
            {
                CreateBetSample(action);
                CreateBetTransactionSamples(action);
            }
        }

        public Bet GetBet()
        {
            return _fakeBet;
        }

        public static Bet CreateBet(BetTypeEnum? typeId, BetStatusEnum? statusId = null)
        {
            var faker = new FakeBet();
            faker.FakeData
                 .RuleFor(x => x.BetStatusId, x => statusId)
                 .RuleFor(x => x.ExternalId, (x, y) =>
                 {
                     var externalId = string.Empty;
                     if (statusId != null && statusId != BetStatusEnum.Reserved)
                         externalId = $"{y.TicketId}-ext";
                     return externalId;
                 })
                 .RuleFor(x => x.BetTypeId, x => typeId)
                 .RuleFor(x => x.WinAmount, (x, y) =>
                 {
                     var amount = 0m;
                     if (statusId == BetStatusEnum.Won || statusId == BetStatusEnum.WonNotPaid)
                     {
                         amount = y.MaxWinning ?? (y.Stake ?? 0) + 100;
                     }
                     return amount;
                 })
                 .RuleFor(x => x.RefundAmount, (x, y) =>
                 {
                     var amount = 0m;
                     if (statusId == BetStatusEnum.Refunded || statusId == BetStatusEnum.Refunded)
                     {
                         amount = y.Stake ?? 0;
                     }
                     return amount;
                 });

            return faker.FakeData.Generate();
        }

        public static BetRequest CreateBetRequest(Bet bet, BetRequestTypeEnum betRequestTypeId)
        {
            var faker = new FakeBetRequest();
            faker.FakeData.RuleFor(x => x.Game, x => GameTypeConverter.BetTypeEnumToGameString(bet.BetTypeId))
                          .RuleFor(x => x.BetRequestTypeId, x => betRequestTypeId)
                          .RuleFor(x => x.BetId, x => bet.BetId)
                          .RuleFor(x => x.TicketId, x => bet.TicketId)
                          .RuleFor(x => x.ExternalId, bet.ExternalId)
                          .RuleFor(x => x.UserId, x => bet.UserId);

            return faker.FakeData.Generate();
        }

        public static WebBetRequest CreateWebBetRequest(BetRequest betRequest, BetSettlementReasonEnum? reasonId)
        {
            var faker = new FakeWebBetRequest(betRequest);
            faker.FakeData.RuleFor(x => x.BetSettlementReasonId, x => (betRequest?.BetRequestTypeId == BetRequestTypeEnum.WebSettleBet) ? reasonId : null);

            return faker.FakeData.Generate();
        }

        public static List<BetTransaction> CreateBetTransactions(Bet bet, params BetTransactionTypeEnum[] transactionTypes)
        {
            var result = new List<BetTransaction>();
            foreach (var typeId in transactionTypes)
            {
                result.Add(CreateBetTransaction(bet, typeId));
            }

            return result;
        }

        public static BetTransaction CreateBetTransaction(Bet bet, BetTransactionTypeEnum typeId, long? betRequestId = null)
        {
            var faker = new FakeBetTransaction(typeId);
            faker.FakeData.RuleFor(x => x.BetTransactionId, x => x.Random.Long(100000, 500000))
                          .RuleFor(x => x.BetId, x => bet?.BetId)
                          .RuleFor(x => x.BetRequestId, x => betRequestId)
                          .RuleFor(x => x.TransactionId, x => x.Random.Long(1000000, 5000000))
                          .RuleFor(x => x.PaymentOrderId, x => x.Random.Long(1000000, 5000000));

            switch (typeId)
            {
                case BetTransactionTypeEnum.Stake:
                    faker.FakeData
                         .RuleFor(x => x.Amount, (x, y) =>
                         {
                             decimal amount;
                             amount = (bet?.Stake ?? 0) * -1;
                             if (amount == 0)
                                 amount = x.Finance.Amount(0.01m, -150);
                             return amount;
                         });
                    break;
                case BetTransactionTypeEnum.Win:
                    faker.FakeData
                         .RuleFor(x => x.Amount, (x, y) =>
                         {
                             decimal amount;
                             amount = bet.WinAmount ?? 0;
                             if (amount == 0)
                                 amount = x.Finance.Amount(bet.Stake ?? 0, bet.MaxWinning ?? ((bet.Stake ?? 0) + 100));
                             return amount;
                         });
                    break;
                case BetTransactionTypeEnum.StakeCompensation:
                    faker.FakeData
                         .RuleFor(x => x.Amount, x =>
                         {
                             decimal amount;
                             amount = bet.RefundAmount ?? 0;
                             if (amount == 0)
                                 amount = x.Finance.Amount(0.01m, (bet.Stake ?? 0) - 0.01m);
                             return amount;
                         });
                    break;
                case BetTransactionTypeEnum.RefundStakeCompensation:
                    faker.FakeData
                         .RuleFor(x => x.Amount, x =>
                         {
                             decimal amount = 0;
                             if ((bet.RefundAmount ?? 0) != 0 && (bet.RefundAmount ?? 0) < (bet.Stake ?? 0))
                                 amount = bet.RefundAmount ?? 0;
                             if (amount == 0 && (bet.Stake ?? 0) > 0)
                                 amount = x.Finance.Amount(0, (bet.Stake ?? 0) - 0.01m) * -1;
                             if (amount == 0)
                                 amount = x.Finance.Amount(0.01m, -150);
                             return amount * -1;
                         });
                    break;
                case BetTransactionTypeEnum.RefundStake:
                    faker.FakeData
                         .RuleFor(x => x.Amount, x =>
                         {
                             decimal amount;
                             amount = (bet.Stake ?? 0);
                             if (amount == 0)
                                 amount = x.Finance.Amount(0.01m, 150);
                             return amount;
                         });
                    break;
                case BetTransactionTypeEnum.TaxStake:
                case BetTransactionTypeEnum.TaxWin:
                    faker.FakeData
                         .RuleFor(x => x.Amount, x => x.Finance.Amount(-0.01m, -20));
                    break;
                case BetTransactionTypeEnum.RefundTaxStake:
                case BetTransactionTypeEnum.RefundTaxWin:
                    faker.FakeData
                         .RuleFor(x => x.Amount, x => x.Finance.Amount(0.01m, 20));
                    break;
            }

            return faker.FakeData.Generate();
        }

        /// <summary>
        /// Get Fake BetTransactions linked in a logical manner with the bet and betRequest
        /// </summary>
        /// <param name="transactionTypes"></param>
        /// <returns></returns>
        public List<BetTransaction> GetBetTransactions(params BetTransactionTypeEnum[] transactionTypes)
        {
            if (transactionTypes != null)
                return _fakeBetTransactions?.Where(x => transactionTypes.Contains((BetTransactionTypeEnum)x.BetTransactionTypeId))?.ToList();
            else
                return _fakeBetTransactions;
        }

        public BetTransaction GetBetTransaction(BetTransactionTypeEnum typeId)
        {
            return GetBetTransactions(typeId).FirstOrDefault();
        }

        public List<BetTransaction> GetLastBetTransactions()
        {
            var lastBetRequestId = _fakeBetTransactions?.Max(x => x.BetRequestId);
            return _fakeBetTransactions.Where(x => x.BetRequestId == lastBetRequestId).ToList();
        }

        public static BetTransaction ConvertToRefund(BetTransaction betTransaction)
        {
            BetTransaction result = null;

            if (betTransaction != null)
            {
                result = new BetTransaction();
                result.BetId = betTransaction.BetId;
                result.Amount = (betTransaction.Amount ?? 0) * -1;
                result.CurrencyCode = betTransaction.CurrencyCode;
                result.RefundBetTransactionId = betTransaction.BetTransactionId;

                switch (betTransaction.BetTransactionTypeId)
                {
                    case BetTransactionTypeEnum.Stake:
                        result.BetTransactionTypeId = BetTransactionTypeEnum.RefundStake;
                        break;
                    case BetTransactionTypeEnum.StakeCompensation:
                        result.BetTransactionTypeId = BetTransactionTypeEnum.RefundStakeCompensation;
                        break;
                    case BetTransactionTypeEnum.Win:
                        result.BetTransactionTypeId = BetTransactionTypeEnum.RefundWin;
                        break;
                    case BetTransactionTypeEnum.TaxStake:
                        result.BetTransactionTypeId = BetTransactionTypeEnum.RefundTaxStake;
                        break;
                    case BetTransactionTypeEnum.TaxWin:
                        result.BetTransactionTypeId = BetTransactionTypeEnum.RefundTaxWin;
                        break;
                    case BetTransactionTypeEnum.RefundStake:
                        result.BetTransactionTypeId = BetTransactionTypeEnum.Stake;
                        break;
                    case BetTransactionTypeEnum.RefundStakeCompensation:
                        result.BetTransactionTypeId = BetTransactionTypeEnum.StakeCompensation;
                        break;
                    case BetTransactionTypeEnum.RefundWin:
                        result.BetTransactionTypeId = BetTransactionTypeEnum.Win;
                        break;
                    case BetTransactionTypeEnum.RefundTaxStake:
                        result.BetTransactionTypeId = BetTransactionTypeEnum.TaxStake;
                        break;
                    case BetTransactionTypeEnum.RefundTaxWin:
                        result.BetTransactionTypeId = BetTransactionTypeEnum.TaxWin;
                        break;
                }
            }
            return result;
        }

        public static List<BetTransaction> ConvertToRefund(List<BetTransaction> betTransactions)
        {
            var result = new List<BetTransaction>();
            foreach (var tran in betTransactions)
            {
                result.Add(ConvertToRefund(tran));
            }

            return result;
        }

        public static BetTypeEnum ParseTypeId(BetHelperAction action)
        {
            BetTypeEnum typeId = Enum.GetName(typeof(BetHelperAction), action).Contains("_Fix_", StringComparison.OrdinalIgnoreCase) ? BetTypeEnum.Fix :
                                 Enum.GetName(typeof(BetHelperAction), action).Contains("_Psip_", StringComparison.OrdinalIgnoreCase) ? BetTypeEnum.PsipTote :
                                 Enum.GetName(typeof(BetHelperAction), action).Contains("_Psr_", StringComparison.OrdinalIgnoreCase) ? BetTypeEnum.PsrTote : default;
            return typeId;
        }

        public static BetStatusEnum? ParseStatusId(BetHelperAction action)
        {
            BetStatusEnum? statusId = null;
            switch (action)
            {
                case BetHelperAction.Create_Fix_Reserved:
                case BetHelperAction.Create_Psr_Reserved:
                case BetHelperAction.Create_Psip_Reserved:
                    statusId = BetStatusEnum.Reserved;
                    break;
                case BetHelperAction.Create_Fix_Placed:
                case BetHelperAction.Create_Psr_Placed:
                case BetHelperAction.Create_Psip_Placed:
                case BetHelperAction.Create_Fix_UndoRefund:
                case BetHelperAction.Create_Psip_UndoRefund:
                case BetHelperAction.Create_Psr_UndoRefund:
                    statusId = BetStatusEnum.Placed;
                    break;
                case BetHelperAction.Create_Fix_Refunded:
                case BetHelperAction.Create_Psr_Refunded:
                case BetHelperAction.Create_Psip_Refunded:
                    statusId = BetStatusEnum.Refunded;
                    break;
                case BetHelperAction.Create_Psr_Refunded_ByStakeCompensation:
                case BetHelperAction.Create_Psip_Refunded_ByStakeCompensation:
                    statusId = BetStatusEnum.Refunded;
                    break;
                case BetHelperAction.Create_Fix_RefundedNotPaid:
                case BetHelperAction.Create_Psr_RefundedNotPaid:
                case BetHelperAction.Create_Psip_RefundedNotPaid:
                    statusId = BetStatusEnum.RefundedNotPaid;
                    break;
                case BetHelperAction.Create_Psr_RefundedNotPaid_ByStakeCompensation:
                case BetHelperAction.Create_Psip_RefundedNotPaid_ByStakeCompensation:
                    statusId = BetStatusEnum.RefundedNotPaid;
                    break;
                case BetHelperAction.Create_Fix_Lost:
                case BetHelperAction.Create_Psr_Lost:
                case BetHelperAction.Create_Psip_Lost:
                    statusId = BetStatusEnum.Lost;
                    break;
                case BetHelperAction.Create_Fix_Won:
                case BetHelperAction.Create_Psr_Won:
                case BetHelperAction.Create_Psip_Won:
                    statusId = BetStatusEnum.Won;
                    break;
                case BetHelperAction.Create_Psr_Won_IncludingStakeCompensation:
                case BetHelperAction.Create_Psip_Won_IncludingStakeCompensation:
                    statusId = BetStatusEnum.Won;
                    break;
                case BetHelperAction.Create_Fix_WonNotPaid:
                case BetHelperAction.Create_Psr_WonNotPaid:
                case BetHelperAction.Create_Psip_WonNotPaid:
                    statusId = BetStatusEnum.WonNotPaid;
                    break;
                case BetHelperAction.Create_Psr_WonNotPaid_IncludingStakeCompensation:
                case BetHelperAction.Create_Psip_WonNotPaid_IncludingStakeCompensation:
                    statusId = BetStatusEnum.WonNotPaid;
                    break;
            }
            return statusId;
        }


        private void CreateBetSample(BetHelperAction action)
        {
            BetTypeEnum? typeId = ParseTypeId(action);
            BetStatusEnum? statusId = ParseStatusId(action);

            _fakeBet = CreateBet(typeId, statusId);


            //alter for Refunded by stake compensation
            if (action == BetHelperAction.Create_Psip_Refunded_ByStakeCompensation ||
                action == BetHelperAction.Create_Psip_RefundedNotPaid_ByStakeCompensation ||
                action == BetHelperAction.Create_Psip_Won_IncludingStakeCompensation ||
                action == BetHelperAction.Create_Psip_WonNotPaid_IncludingStakeCompensation ||

                action == BetHelperAction.Create_Psr_Refunded_ByStakeCompensation ||
                action == BetHelperAction.Create_Psr_RefundedNotPaid_ByStakeCompensation ||
                action == BetHelperAction.Create_Psr_Won_IncludingStakeCompensation ||
                action == BetHelperAction.Create_Psr_WonNotPaid_IncludingStakeCompensation)
            {
                _fakeBet.RefundAmount = Math.Round((decimal)(new Random().NextDouble()) * ((_fakeBet.Stake ?? 0) - 0.01m) + 0.01m, 2);
            }

        }

        private void CreateBetTransactionSamples(BetHelperAction action)
        {
            var requestId = new Random().Next(1000000, 5000000);
            switch (action)
            {
                case BetHelperAction.Create_Fix_Reserved:
                case BetHelperAction.Create_Psr_Reserved:
                case BetHelperAction.Create_Psip_Reserved:
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Stake, requestId + 1));
                    break;
                case BetHelperAction.Create_Fix_Placed:
                case BetHelperAction.Create_Psr_Placed:
                case BetHelperAction.Create_Psip_Placed:
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Stake, requestId + 1));
                    break;
                case BetHelperAction.Create_Fix_Refunded:
                case BetHelperAction.Create_Psr_Refunded:
                case BetHelperAction.Create_Psip_Refunded:
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Stake, requestId + 1));
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.RefundStake, requestId + 2));
                    break;
                case BetHelperAction.Create_Fix_RefundedNotPaid:
                case BetHelperAction.Create_Psr_RefundedNotPaid:
                case BetHelperAction.Create_Psip_RefundedNotPaid:
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Stake, requestId + 1));
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.RefundStake, requestId + 2));
                    break;
                case BetHelperAction.Create_Psr_Refunded_ByStakeCompensation:
                case BetHelperAction.Create_Psip_Refunded_ByStakeCompensation:
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Stake, requestId + 1));
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.StakeCompensation, requestId + 2));
                    break;
                case BetHelperAction.Create_Psr_RefundedNotPaid_ByStakeCompensation:
                case BetHelperAction.Create_Psip_RefundedNotPaid_ByStakeCompensation:
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Stake, requestId + 1));
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.StakeCompensation, requestId + 2));
                    break;
                case BetHelperAction.Create_Fix_Won:
                case BetHelperAction.Create_Psr_Won:
                case BetHelperAction.Create_Psip_Won:
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Stake, requestId + 1));
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Win, requestId + 2));
                    break;
                case BetHelperAction.Create_Fix_WonNotPaid:
                case BetHelperAction.Create_Psr_WonNotPaid:
                case BetHelperAction.Create_Psip_WonNotPaid:
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Stake, requestId + 1));
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Win, requestId + 2));
                    break;
                case BetHelperAction.Create_Psr_Won_IncludingStakeCompensation:
                case BetHelperAction.Create_Psip_Won_IncludingStakeCompensation:
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Stake, requestId + 1));
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Win, requestId + 2));
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.StakeCompensation, requestId + 2));
                    break;
                case BetHelperAction.Create_Psr_WonNotPaid_IncludingStakeCompensation:
                case BetHelperAction.Create_Psip_WonNotPaid_IncludingStakeCompensation:
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Stake, requestId + 1));
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Win, requestId + 2));
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.StakeCompensation, requestId + 2));
                    break;
                case BetHelperAction.Create_Fix_Lost:
                case BetHelperAction.Create_Psr_Lost:
                case BetHelperAction.Create_Psip_Lost:
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Stake));
                    break;
                case BetHelperAction.Create_Fix_UndoRefund:
                case BetHelperAction.Create_Psr_UndoRefund:
                case BetHelperAction.Create_Psip_UndoRefund:
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Stake, requestId + 1));
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.RefundStake, requestId + 2));
                    _fakeBetTransactions.Add(CreateBetTransaction(_fakeBet, BetTransactionTypeEnum.Stake, requestId + 3));
                    break;
            }

        }

        /// <summary>
        /// Clear the fake bet in order to use as a param to the BetService methods
        /// </summary>
        /// <param name="clear">pass true if you want to clean fake bet</param>
        /// <returns></returns>
        public static Bet Clear(Bet bet)
        {
            var clone = bet?.Clone();

            if (clone != null)
            {
                clone.BetStatusId = null;
                clone.BetId = null;
            }
            return clone;
        }

        public static BetRequest Clear(BetRequest betRequest)
        {
            var clone = betRequest?.Clone();
            if (clone != null)
            {
                clone.BetRequestId = null;
                clone.BetId = null;
            }
            return clone;
        }

        public static BetTransaction Clear(BetTransaction betTransaction)
        {
            var clone = betTransaction?.Clone();
            if (clone != null)
            {
                clone.BetTransactionId = null;
                clone.BetRequestId = null;
                clone.BetId = null;
                clone.TransactionId = null;
                clone.PaymentOrderId = null;
            }
            return clone;
        }

        public static List<BetTransaction> Clear(List<BetTransaction> betTransactions)
        {
            var cloneList = new List<BetTransaction>();
            foreach (var tran in betTransactions)
            {
                cloneList.Add(Clear(tran));
            }

            return cloneList;
        }
    }
}