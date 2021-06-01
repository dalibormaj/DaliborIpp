using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sks365.Ippica.Application.Services.Abstraction;
using Sks365.Ippica.Application.Utility;
using Sks365.Ippica.Application.Utility.EmailSender;
using Sks365.Ippica.Application.Utility.OperationRecorder;
using Sks365.Ippica.Application.Utility.Rules;
using Sks365.Ippica.Common.Config.Abstraction;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.DataAccess;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.Ippica.Domain.Utility;
using Sks365.SessionTracker.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sks365.Ippica.Application.Services
{
    public class BetService : IBetService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPaymentOrderService _paymentOrderService;
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly IOperationRecorder _operationRecorder;
        private readonly ISessionTracker _sessionTracker;
        private readonly ILogger<BetService> _logger;
        private readonly IAppSettings _appSettings;


        public BetService(IHttpContextAccessor context, IPaymentOrderService paymentOrderService, IUserService userService,
                          IEmailSender emailSender, IOperationRecorder operationTracker, ISessionTracker sessionTracker,
                          ILogger<BetService> logger, IAppSettings appSettings)
        {
            _serviceProvider = context.HttpContext.RequestServices;
            _paymentOrderService = paymentOrderService;
            _userService = userService;
            _emailSender = emailSender;
            _sessionTracker = sessionTracker;
            _logger = logger;
            _operationRecorder = operationTracker;
            _appSettings = appSettings;
        }

        public async Task<Bet> CancelStake(BetRequest betRequest, List<BetTransaction> betTransactions = null, bool processTransactionsAsPending = false)
        {
            var operationRecorderExecutor = _operationRecorder.CreateExecutor(betRequest.TicketId, betRequest.ExternalId, (BetRequestTypeEnum)betRequest.BetRequestTypeId);
            using (operationRecorderExecutor)
            {
                await operationRecorderExecutor.OperationStart();

                User user = GetUser(betRequest);
                var userLanguageId = user.GetUserLanguage();

                ValidateBetRequest(betRequest, user);

                var targetBetStatus = processTransactionsAsPending ? BetStatusEnum.RefundedNotPaid : BetStatusEnum.Refunded;

                var betRule = new BetRule(_serviceProvider, betRequest, userLanguageId);
                var ruleResult = betRule.CurrentStatus(null, BetStatusEnum.Reserved, BetStatusEnum.Placed, BetStatusEnum.RefundedNotPaid, BetStatusEnum.Refunded)
                                        .NewStatus(targetBetStatus)
                                        .Execute();

                var betToSave = ruleResult.Bet;
                var isRetry = ruleResult.IsRetry;

                //fake bet. It happens that while the IIS is down, usually during the release process,
                //MST still tries to hit an API even they can't reach us since we are offline. So, in this case, the server will
                //respond with 503 and due to their flow that every failed reservation should be rolled back, they will 
                //start trying. Just upon we become reachable/online, we will be able to register these rollback requests (in logs) but 
                //also we'll notice that there are no reservation requests. In normal cases, this rollback request should be rejected since
                //we have nothing to refund (rollback must be after reservation). In order to avoid infinitive attempts for refunding something 
                //that cannot be refunded the solution is to create a fake bet with 0 amount, but only in case of Rollback requests
                if (betToSave == null && !string.IsNullOrEmpty(betRequest?.TicketId) && (betRequest?.BetRequestTypeId == BetRequestTypeEnum.WebRollbackBet ||
                                                                                         betRequest?.BetRequestTypeId == BetRequestTypeEnum.ShopRollbackBet))
                {
                    betToSave = new Bet()
                    {
                        UserId = user?.UserId ?? -1,
                        Amount = 0,
                        Stake = 0,
                        RefundAmount = 0,
                        WinAmount = 0,
                        CurrencyId = user?.AccountCurrencyId ?? CurrencyEnum.EUR,
                        BetStatusId = targetBetStatus,
                        TicketId = betRequest.TicketId,
                        ExternalId = betRequest.ExternalId,
                        BetTypeId = GameTypeConverter.GameToBetTypeEnum(betRequest.Game)
                    };
                }

                //very important: must have validation
                betToSave.ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.BetNotFound))
                         .ThrowIf(x => x.BetTypeId != GameTypeConverter.GameToBetTypeEnum(betRequest.Game), new IppicaException(ReturnCodeEnum.InvalidBetType));

                //change the type if amount is less than stake
                betTransactions?.Where(x => x.BetTransactionTypeId == BetTransactionTypeEnum.RefundStake && Math.Abs(x.Amount ?? 0) < Math.Abs(betToSave.Stake ?? 0))
                                .ToList()
                                .ForEach(x => x.BetTransactionTypeId = BetTransactionTypeEnum.StakeCompensation);

                if (!isRetry)
                {
                    SetUser(betRequest, betToSave, user);

                    //If there are no transactions to process try to find manually.
                    if ((betTransactions?.Count ?? 0) == 0 && betToSave?.BetId != null)
                    {
                        var unitOfWork = _serviceProvider.GetService(typeof(IMstUnitOfWork)) as IMstUnitOfWork;
                        using (unitOfWork)
                        {
                            var refundTran = unitOfWork.BetRepository.GetActiveTransaction((long)betToSave.BetId, BetTransactionTypeEnum.Stake, convertToRefund: true);
                            if (refundTran != null)
                                betTransactions = new List<BetTransaction>() { refundTran };
                        }
                    }

                    var betTransactionRule = new BetTransactionRule(_serviceProvider, betToSave.BetId, betTransactions, userLanguageId);
                    var betTransactionsToSave = betTransactionRule.UniqueTypes()
                                                                  .SameAsPreviousIf(BetStatusEnum.RefundedNotPaid)
                                                                  .SupportedTypes(BetTransactionTypeEnum.RefundStake,
                                                                                  BetTransactionTypeEnum.RefundTaxStake,
                                                                                  BetTransactionTypeEnum.StakeCompensation)
                                                                  .Execute();

                    var stakeAmount = betToSave?.Stake ?? 0;
                    var refundStake = betTransactionsToSave?.Find(x => x != null && x.BetTransactionTypeId == BetTransactionTypeEnum.RefundStake);
                    var stakeCompensation = betTransactionsToSave?.Find(x => x != null && x.BetTransactionTypeId == BetTransactionTypeEnum.StakeCompensation);

                    var refundStakeAmount = refundStake?.Amount ?? 0;
                    var stakeCompensationAmount = stakeCompensation?.Amount ?? 0;
                    var refundAmount = Math.Abs(refundStakeAmount) + Math.Abs(stakeCompensationAmount);

                    //take stakeAmount as refundStakeAmount only if there is no stake to be refunded.
                    //It's solution for a problem when reservation fails and when ticket is created
                    //but with no transaction or in cases when we have just rollbacks with no proper reservation
                    //logged before. Rollback request (cancel stake), in that case, will set the same value 
                    //as stake amount even we don't have transaction to refund.
                    betToSave.RefundAmount = refundAmount != 0 ? refundAmount : stakeAmount;
                    betToSave.ThrowIf(x => (x?.Stake ?? 0) > 0 && (x?.RefundAmount ?? 0) <= 0, new IppicaException(ReturnCodeEnum.BetInvalid, "RefundAmount cannot be zero or less"));

                    betToSave = await SaveAll(betRequest, betToSave, betTransactionsToSave, processTransactionsAsPending);

                    return betToSave;
                }
                else
                {
                    var isProcessed = IsProcessed(betRequest.TicketId, betRequest.ExternalId, null, betTransactions);

                    if (isProcessed)
                        return betToSave;
                    else
                        throw new IppicaException(ReturnCodeEnum.BetProcessedDataMismatched, userLanguageId);
                }
            }
        }

        public async Task<Bet> UndoCancelStake(BetRequest betRequest, List<BetTransaction> betTransactions = null)
        {
            var operationRecorderExecutor = _operationRecorder.CreateExecutor(betRequest.TicketId, betRequest.ExternalId, (BetRequestTypeEnum)betRequest.BetRequestTypeId);
            using (operationRecorderExecutor)
            {
                await operationRecorderExecutor.OperationStart();
                User user = GetUser(betRequest);
                var userLanguageId = user.GetUserLanguage();

                ValidateBetRequest(betRequest, user);

                var targetBetStatus = BetStatusEnum.Placed;

                var betRule = new BetRule(_serviceProvider, betRequest, userLanguageId);
                var ruleResult = betRule.CurrentStatus(BetStatusEnum.Refunded, targetBetStatus)
                                        .NewStatus(targetBetStatus)
                                        .Execute();

                var betToSave = ruleResult.Bet;
                var isRetry = ruleResult.IsRetry;

                //change the type if amount is less than stake
                betTransactions?.Where(x => x.BetTransactionTypeId == BetTransactionTypeEnum.Stake && Math.Abs(x.Amount ?? 0) < Math.Abs(betToSave.Stake ?? 0))
                                .ToList()
                                .ForEach(x => x.BetTransactionTypeId = BetTransactionTypeEnum.RefundStakeCompensation);

                if (!isRetry)
                {
                    SetUser(betRequest, betToSave, user);

                    var betTransactionRule = new BetTransactionRule(_serviceProvider, betToSave.BetId, betTransactions, userLanguageId);
                    var betTransactionsToSave = betTransactionRule.UniqueTypes()
                                                                  .SupportedTypes(BetTransactionTypeEnum.Stake,
                                                                                  BetTransactionTypeEnum.TaxStake,
                                                                                  BetTransactionTypeEnum.RefundStakeCompensation)
                                                                  .CheckSufficientFunds(email: new Email()
                                                                  {
                                                                      BetRequestTypeId = betRequest.BetRequestTypeId,
                                                                      BetId = betToSave.BetId,
                                                                      TicketId = betToSave.TicketId,
                                                                      ExternalId = betToSave.ExternalId,
                                                                      From = "ippica@sks365.com",
                                                                      To = _appSettings?.EmailSender?.To,
                                                                      Cc = _appSettings?.EmailSender?.Cc,
                                                                      Subject = $"Ippica ticket {betToSave.ExternalId} cannot be processed",
                                                                      Message = EmailTemplate.GetUndoCancelEmailText(betToSave)
                                                                  })
                                                                  .RefundPrevious()
                                                                  .MustContainOneOf(BetTransactionTypeEnum.Stake, BetTransactionTypeEnum.RefundStakeCompensation)
                                                                  .Execute();

                    betToSave.RefundAmount = 0;

                    betToSave = await SaveAll(betRequest, betToSave, betTransactionsToSave);

                    return betToSave;
                }
                else
                {
                    var isProcessed = IsProcessed(betRequest.TicketId, betRequest.ExternalId, null, betTransactions);

                    if (isProcessed)
                        return betToSave;
                    else
                        throw new IppicaException(ReturnCodeEnum.BetProcessedDataMismatched, userLanguageId);
                }
            }
        }


        public async Task<Bet> CancelWin(BetRequest betRequest, List<BetTransaction> betTransactions = null)
        {
            var operationRecorderExecutor = _operationRecorder.CreateExecutor(betRequest.TicketId, betRequest.ExternalId, (BetRequestTypeEnum)betRequest.BetRequestTypeId);
            using (operationRecorderExecutor)
            {
                await operationRecorderExecutor.OperationStart();
                User user = GetUser(betRequest);
                var userLanguageId = user.GetUserLanguage();

                ValidateBetRequest(betRequest, user);

                var targetBetStatus = BetStatusEnum.Placed;

                var betRule = new BetRule(_serviceProvider, betRequest, userLanguageId);
                var ruleResult = betRule.CurrentStatus(BetStatusEnum.Won, BetStatusEnum.WonNotPaid, targetBetStatus)
                                        .NewStatus(targetBetStatus)
                                        .Execute();

                var betToSave = ruleResult.Bet;
                var isRetry = ruleResult.IsRetry;

                if (!isRetry)
                {
                    SetUser(betRequest, betToSave, user);

                    //If there are no transactions to process try to find manually.
                    if ((betTransactions?.Count ?? 0) == 0 && betToSave?.BetId != null)
                    {
                        var unitOfWork = _serviceProvider.GetService(typeof(IMstUnitOfWork)) as IMstUnitOfWork;
                        using (unitOfWork)
                        {
                            var refundTran1 = unitOfWork.BetRepository.GetActiveTransaction((long)betToSave.BetId, BetTransactionTypeEnum.Win, convertToRefund: true);
                            var refundTran2 = unitOfWork.BetRepository.GetActiveTransaction((long)betToSave.BetId, BetTransactionTypeEnum.StakeCompensation, convertToRefund: true);
                            betTransactions = new List<BetTransaction>();
                            if (refundTran1 != null)
                                betTransactions.Add(refundTran1);
                            if (refundTran2 != null)
                                betTransactions.Add(refundTran2);
                        }
                    }

                    var betTransactionRule = new BetTransactionRule(_serviceProvider, betToSave.BetId, betTransactions, userLanguageId);
                    var betTransactionsToSave = betTransactionRule.UniqueTypes()
                                                                  .SupportedTypes(BetTransactionTypeEnum.RefundWin,
                                                                                  BetTransactionTypeEnum.RefundTaxWin,
                                                                                  BetTransactionTypeEnum.RefundStakeCompensation)
                                                                  .RefundPrevious()
                                                                  .MustContain(BetTransactionTypeEnum.RefundWin)
                                                                  .Execute();

                    var refundWin = betTransactionsToSave.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.RefundWin);
                    var refundStakeCompensation = betTransactionsToSave.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.RefundStakeCompensation);

                    betToSave.WinAmount = 0;
                    betToSave.RefundAmount = 0;
                    betToSave.TaxWin = 0;

                    betToSave = await SaveAll(betRequest, betToSave, betTransactionsToSave);

                    return betToSave;
                }
                else
                {
                    var isProcessed = IsProcessed(betRequest.TicketId, betRequest.ExternalId, null, betTransactions);

                    if (isProcessed)
                        return betToSave;
                    else
                        throw new IppicaException(ReturnCodeEnum.BetProcessedDataMismatched, userLanguageId);
                }
            }
        }

        public async Task<Bet> Place(BetRequest betRequest, Bet bet, List<BetTransaction> betTransactions)
        {
            var operationRecorderExecutor = _operationRecorder.CreateExecutor(betRequest.TicketId, betRequest.ExternalId, (BetRequestTypeEnum)betRequest.BetRequestTypeId);
            using (operationRecorderExecutor)
            {
                await operationRecorderExecutor.OperationStart();

                User user = GetUser(betRequest, bet);
                var userLanguageId = user.GetUserLanguage();

                ValidateBetRequest(betRequest, user);

                var targetBetStatus = BetStatusEnum.Placed;

                var betRule = new BetRule(_serviceProvider, betRequest, bet, userLanguageId);
                var ruleResult = betRule.CurrentStatus(BetStatusEnum.Reserved, targetBetStatus)
                                        .NewStatus(targetBetStatus)
                                        .Execute();

                var betToSave = ruleResult.Bet;
                var isRetry = ruleResult.IsRetry;

                if (!isRetry)
                {
                    SetUser(betRequest, betToSave, user);

                    var betTransactionRule = new BetTransactionRule(_serviceProvider, betToSave.BetId, betTransactions, userLanguageId);
                    var betTransactionsToSave = betTransactionRule.UniqueTypes()
                                                                  .SameAsPreviousIf(BetStatusEnum.Reserved)
                                                                  .SupportedTypes(BetTransactionTypeEnum.Stake,
                                                                                  BetTransactionTypeEnum.TaxStake)
                                                                  .MustContain(BetTransactionTypeEnum.Stake)
                                                                  .Execute();

                    betToSave = await SaveAll(betRequest, betToSave, betTransactionsToSave);

                    return betToSave;
                }
                else
                {
                    var isProcessed = IsProcessed(betRequest.TicketId, betRequest.ExternalId, bet, betTransactions);

                    if (isProcessed)
                        return betToSave;
                    else
                        throw new IppicaException(ReturnCodeEnum.BetProcessedDataMismatched, userLanguageId);
                }
            }
        }

        public async Task<Bet> Reopen(BetRequest betRequest)
        {
            User user = GetUser(betRequest);
            var userLanguageId = user.GetUserLanguage();

            ValidateBetRequest(betRequest, user);

            var targetBetStatus = BetStatusEnum.Placed;

            var betRule = new BetRule(_serviceProvider, betRequest, userLanguageId);
            var ruleResult = betRule.CurrentStatus(BetStatusEnum.Lost, targetBetStatus)
                                    .NewStatus(targetBetStatus)
                                    .Execute();

            var betToSave = ruleResult.Bet;
            var isRetry = ruleResult.IsRetry;

            if (!isRetry)
            {
                var ownerUserId = betToSave?.UserId;
                SetUser(betRequest, betToSave, user);

                betToSave = await SaveAll(betRequest, betToSave);

                return betToSave;
            }
            else
            {
                return betToSave;
            }
        }

        public async Task<Bet> Reserve(BetRequest betRequest, Bet bet, List<BetTransaction> betTransactions)
        {
            var operationRecorderExecutor = _operationRecorder.CreateExecutor(betRequest.TicketId, betRequest.ExternalId, (BetRequestTypeEnum)betRequest.BetRequestTypeId);
            using (operationRecorderExecutor)
            {
                await operationRecorderExecutor.OperationStart();

                User user = GetUser(betRequest, bet);
                var userLanguageId = user.GetUserLanguage();

                ValidateBetRequest(betRequest, user);

                var targetBetStatus = BetStatusEnum.Reserved;
                var betRule = new BetRule(_serviceProvider, betRequest, bet, userLanguageId);
                var ruleResult = betRule.CurrentStatus(null, targetBetStatus)
                                        .NewStatus(targetBetStatus)
                                        .Execute();

                var betToSave = ruleResult.Bet;
                var isRetry = ruleResult.IsRetry;

                if (!isRetry)
                {
                    SetUser(betRequest, betToSave, user);
                    var betTransactionRule = new BetTransactionRule(_serviceProvider, betToSave.BetId, betTransactions, userLanguageId);
                    var betTransactionsToSave = betTransactionRule.UniqueTypes()
                                                                  .SupportedTypes(BetTransactionTypeEnum.Stake,
                                                                                  BetTransactionTypeEnum.TaxStake)
                                                                  .MustContain(BetTransactionTypeEnum.Stake)
                                                                  .CheckSufficientFunds(betToSave.UserId)
                                                                  .Execute();

                    betToSave = await SaveAll(betRequest, betToSave, betTransactions, processTransactionsAsPending: true);

                    return betToSave;
                }
                else
                {
                    var isProcessed = IsProcessed(betRequest.TicketId, betRequest.ExternalId, bet, betTransactions);

                    if (isProcessed)
                        return betToSave;
                    else
                        throw new IppicaException(ReturnCodeEnum.BetProcessedDataMismatched, userLanguageId);
                }
            }
        }

        public async Task<Bet> SettleLoss(BetRequest betRequest)
        {
            User user = GetUser(betRequest);
            var userLanguageId = user.GetUserLanguage();

            ValidateBetRequest(betRequest, user);

            var targetBetStatus = BetStatusEnum.Lost;

            var betRule = new BetRule(_serviceProvider, betRequest, userLanguageId);
            var ruleResult = betRule.CurrentStatus(BetStatusEnum.Placed, targetBetStatus)
                                    .NewStatus(targetBetStatus)
                                    .Execute();

            var betToSave = ruleResult.Bet;
            var isRetry = ruleResult.IsRetry;

            if (!isRetry)
            {
                SetUser(betRequest, betToSave, user);
                betToSave = await SaveAll(betRequest, betToSave);
            }

            return betToSave;
        }

        public async Task<Bet> SettleWin(BetRequest betRequest, List<BetTransaction> betTransactions, bool processTransactionsAsPending = false)
        {
            var operationRecorderExecutor = _operationRecorder.CreateExecutor(betRequest.TicketId, betRequest.ExternalId, (BetRequestTypeEnum)betRequest.BetRequestTypeId);
            using (operationRecorderExecutor)
            {
                await operationRecorderExecutor.OperationStart();

                User user = GetUser(betRequest);
                var userLanguageId = user.GetUserLanguage();

                ValidateBetRequest(betRequest, user);

                var targetBetStatus = processTransactionsAsPending ? BetStatusEnum.WonNotPaid : BetStatusEnum.Won;

                var betRule = new BetRule(_serviceProvider, betRequest, userLanguageId);
                var ruleResult = betRule.CurrentStatus(BetStatusEnum.Placed, BetStatusEnum.WonNotPaid, BetStatusEnum.Won)
                                        .NewStatus(targetBetStatus)
                                        .Execute();

                var betToSave = ruleResult.Bet;
                var isRetry = ruleResult.IsRetry;

                if (!isRetry)
                {
                    SetUser(betRequest, betToSave, user);

                    var betTransactionRule = new BetTransactionRule(_serviceProvider, betToSave.BetId, betTransactions, userLanguageId);
                    var betTransactionsToSave = betTransactionRule.UniqueTypes()
                                                                  .SameAsPreviousIf(BetStatusEnum.WonNotPaid)
                                                                  .SupportedTypes(BetTransactionTypeEnum.Win,
                                                                                  BetTransactionTypeEnum.TaxWin,
                                                                                  BetTransactionTypeEnum.TaxStake,
                                                                                  BetTransactionTypeEnum.StakeCompensation)
                                                                  .MustContain(BetTransactionTypeEnum.Win)
                                                                  .Execute();

                    var win = betTransactionsToSave.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.Win);
                    var stakeCompensation = betTransactionsToSave.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.StakeCompensation);

                    betToSave.WinAmount = win.Amount;
                    betToSave.RefundAmount = stakeCompensation?.Amount ?? 0;

                    betToSave = await SaveAll(betRequest, betToSave, betTransactionsToSave, processTransactionsAsPending);

                    return betToSave;
                }
                else
                {
                    //check the data
                    var isProcessed = IsProcessed(betRequest.TicketId, betRequest.ExternalId, null, betTransactions);

                    if (isProcessed)
                        return betToSave;
                    else
                        throw new IppicaException(ReturnCodeEnum.BetProcessedDataMismatched, userLanguageId);
                }
            }
        }

        private async Task<Bet> SaveAll(BetRequest betRequest, Bet bet, List<BetTransaction> betTransactions = null, bool processTransactionsAsPending = false)
        {
            var unitOfWork = _serviceProvider.GetService(typeof(IMstUnitOfWork)) as IMstUnitOfWork;
            using (unitOfWork)
            {
                //get the values before updating the bet. It'll be used if something happens 
                //and to rollback the bet to the previuos state
                var tempBet = bet?.BetId == null ? bet.Clone() : unitOfWork.BetRepository.GetBet((long)bet.BetId);

                using (unitOfWork.BeginTransaction())
                {
                    bet = unitOfWork.BetRepository.SaveBet(bet);
                    betRequest.BetId = bet.BetId;
                    betRequest.UserId = bet.UserId;
                    betRequest.TicketId = string.IsNullOrEmpty(betRequest.TicketId) && !string.IsNullOrEmpty(bet?.TicketId) ? bet.TicketId : betRequest.TicketId;
                    betRequest.ExternalId = string.IsNullOrEmpty(betRequest.ExternalId) && !string.IsNullOrEmpty(bet?.ExternalId) ? bet.ExternalId : betRequest.ExternalId;
                    betRequest = unitOfWork.BetRepository.SaveBetRequest(betRequest);

                    unitOfWork.Commit();
                }

                try
                {
                    betTransactions = betTransactions ?? new List<BetTransaction>();
                    foreach (var transaction in betTransactions)
                    {
                        if (transaction != null)
                        {
                            transaction.BetId = bet.BetId;
                            transaction.BetRequestId = betRequest.BetRequestId;

                            var newTran = await _paymentOrderService.ProcessTransaction(transaction, (int)betRequest.UserId, (BookmakerEnum)bet.BookmakerId, betRequest.Ip, processTransactionsAsPending);
                            newTran = unitOfWork.BetRepository.SaveBetTransaction(newTran);

                            if (newTran.PaymentOrderId != null)
                                await _paymentOrderService.UpdateThirdPartyId((long)newTran.PaymentOrderId, newTran.BetTransactionId?.ToString(), (int)betRequest.UserId, (BookmakerEnum)bet.BookmakerId, betRequest.Ip);
                        }
                    }
                }
                catch
                {
                    //Save as error first, just to be aware that error has happened
                    //This value will be stored in History.Bet after next update
                    bet.BetStatusId = BetStatusEnum.Error;
                    bet = unitOfWork.BetRepository.SaveBet(bet);

                    //Return the previous state (in order to let RETRY to fix the problem)
                    //NOTE: It won't fix RETRY ATTEMPTS for ReservationBet if reservation failed.
                    //Luckely MST don't do reservation retry but RollbackBet instead,
                    //in order to fix the failed reservation... so we don't need to cover that problem for now.
                    //Retry for ReservationBet won't work since the transaction is not created and retry will
                    //fail with BetProcessedDataMismatched.
                    bet = unitOfWork.BetRepository.SaveBet(tempBet);
                    throw;
                }
                return bet;
            }
        }

        private User GetUser(BetRequest betRequest, Bet bet)
        {
            User user = null;
            if (betRequest.UserId != null)
            {
                user = _userService.GetUser((int)betRequest.UserId);
            }

            if (user == null && !string.IsNullOrEmpty(betRequest.Session))
            {
                user = _userService.GetUser(betRequest.Session);
            }

            if (user == null && bet?.UserId != null)
            {
                user = _userService.GetUser((int)bet.UserId);
            }

            if (user == null && bet?.UserId == null && (!string.IsNullOrEmpty(betRequest?.ExternalId) || !string.IsNullOrEmpty(betRequest.TicketId)))
            {
                var unitOfWork = _serviceProvider.GetService(typeof(IMstUnitOfWork)) as IMstUnitOfWork;
                using (unitOfWork)
                {
                    //Try with external Id
                    var betDb = !string.IsNullOrEmpty(betRequest?.ExternalId) ? unitOfWork.BetRepository.GetBetByExternalId(betRequest.ExternalId) : null;

                    //Try with Ticket Id
                    if (betDb == null)
                        betDb = !string.IsNullOrEmpty(betRequest?.TicketId) ? unitOfWork.BetRepository.GetBet(betRequest.TicketId) : null;

                    if (betDb != null)
                        user = _userService.GetUser((int)betDb.UserId);
                }
            }

            if (user == null && !string.IsNullOrEmpty(betRequest.WebBetRequest?.UserAccount) && betRequest.WebBetRequest.UserAccount.All(char.IsDigit))
            {
                var userId = Convert.ToInt32(betRequest.WebBetRequest.UserAccount);
                user = _userService.GetUser(userId)
                                   .ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.UserNotFound, $"User {betRequest.WebBetRequest?.UserAccount} cannot be found"));
            }

            if (user == null && !string.IsNullOrEmpty(betRequest.WebBetRequest?.UserAccount) && betRequest.WebBetRequest.UserAccount.All(x => !char.IsDigit(x)) && bet?.BookmakerId != null)
            {
                user = _userService.GetUser(betRequest.WebBetRequest.UserAccount, (BookmakerEnum)bet.BookmakerId)
                                   .ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.UserNotFound, $"User {betRequest.WebBetRequest?.UserAccount} cannot be found"));
            }

            //Get language
            if (user != null)
            {
                user.AdditionalData = new List<UserAdditionalData>() { _userService.GetUserAdditionalDataValue((int)user.UserId, UserDataTypeEnum.Language) };
            }

            return user ?? new User();
        }

        private User GetUser(BetRequest betRequest)
        {
            return GetUser(betRequest, null);
        }

        private void SetUser(BetRequest betRequest, Bet bet, User user)
        {
            if (user?.UserId == null)
                throw new IppicaException(ReturnCodeEnum.UserNotFound);

            if ((bet?.UserId != null && bet.UserId != user.UserId) ||
                (betRequest?.UserId != null && betRequest.UserId != user.UserId))
            {
                throw new IppicaException(ReturnCodeEnum.TicketDoesNotBelongToUser);
            }

            betRequest.UserId = user.UserId;
            bet.UserId = user.UserId;
            bet.BookmakerId = user.BookmakerId;
        }

        /// <summary>
        /// Base BetRequest validation. 
        /// Forbid mixing requests between different bookmakers
        /// </summary>
        /// <param name="betRequest"></param>
        /// <param name="user"></param>
        private void ValidateBetRequest(BetRequest betRequest, User user)
        {
            betRequest.ThrowIf(x => betRequest?.BetRequestTypeId != null &&
                                    user?.BookmakerId != null &&
                                    user.BookmakerId != BookmakerEnum.IT_SHOP &&
                                    new HashSet<BetRequestTypeEnum>
                                        {
                                            BetRequestTypeEnum.ShopReserveBet,
                                            BetRequestTypeEnum.ShopPlaceBet,
                                            BetRequestTypeEnum.ShopCancelBet,
                                            BetRequestTypeEnum.ShopRollbackBet,
                                            BetRequestTypeEnum.ShopSettleBet,
                                            BetRequestTypeEnum.PayBet
                                        }.Contains((BetRequestTypeEnum)betRequest.BetRequestTypeId), new IppicaException(ReturnCodeEnum.RequestNotAllowedForSelectedBookmaker));

            betRequest.ThrowIf(x => betRequest?.BetRequestTypeId != null &&
                                    user.BookmakerId != null &&
                                    user.BookmakerId != BookmakerEnum.IT &&
                                    new HashSet<BetRequestTypeEnum>
                                        {
                                            BetRequestTypeEnum.WebReserveBet,
                                            BetRequestTypeEnum.WebPlaceBet,
                                            BetRequestTypeEnum.WebRollbackBet,
                                            BetRequestTypeEnum.WebSettleBet
                                        }.Contains((BetRequestTypeEnum)betRequest.BetRequestTypeId), new IppicaException(ReturnCodeEnum.RequestNotAllowedForSelectedBookmaker));
        }

        private bool IsProcessed(string ticketId, string externalId, Bet bet, List<BetTransaction> betTransactions)
        {
            var unitOfWork = _serviceProvider.GetService(typeof(IMstUnitOfWork)) as IMstUnitOfWork;
            using (unitOfWork)
            {
                var processed = true;

                var betDb = string.IsNullOrEmpty(externalId) ?
                            unitOfWork.BetRepository.GetBet(ticketId) :
                            unitOfWork.BetRepository.GetBetByExternalId(externalId);

                processed = betDb != null;
                if (!processed)
                    return processed;

                //check bet (must be the same)
                if (bet != null)
                {
                    processed = betDb.Compare(bet);
                    if (!processed)
                        return processed;
                }

                //check last bet transactions (must be the same)
                if ((betTransactions?.Count ?? 0) > 0)
                {
                    var lastBetTransactions = unitOfWork.BetRepository.GetLastBetTransactions((long)betDb.BetId);
                    processed = (lastBetTransactions?.Count ?? 0) == (betTransactions?.Count ?? 0);

                    if (!processed)
                        return processed;

                    foreach (var transaction in betTransactions)
                    {
                        var transactionDb = lastBetTransactions.Find(x => x.BetTransactionTypeId == transaction.BetTransactionTypeId);
                        processed = transactionDb != null;
                        if (!processed) break;

                        processed = transactionDb.Compare(transaction);
                        if (!processed) break;
                    }
                }

                return processed;
            }
        }
    }
}