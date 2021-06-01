using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sks365.Ippica.Application.Services.Abstraction;
using Sks365.Ippica.Application.Utility;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.DataAccess;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.Ippica.Domain.Utility;
using Sks365.Payments.WebApi.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sks365.Ippica.Application.Services
{
    public class PaymentOrderService : IPaymentOrderService
    {
        private readonly IPaymentClient _paymentClient;
        private readonly ITransactionClient _transactionClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentOrderService> _logger;

        public PaymentOrderService(IPaymentClient paymentClient, ITransactionClient transactionClient,
                                   IHttpContextAccessor context, ILogger<PaymentOrderService> logger)
        {
            _paymentClient = paymentClient;
            _transactionClient = transactionClient;
            _serviceProvider = context.HttpContext.RequestServices;
            _logger = logger;
        }

        public PaymentOrder GetPaymentOrder(long paymentOrderId)
        {
            var unitOfWork = _serviceProvider.GetService(typeof(IIsbetsUnitOfWork)) as IIsbetsUnitOfWork;
            using (unitOfWork)
            {
                var paymentOrder = unitOfWork.PaymentOrderRepository.GetPaymentOrder(paymentOrderId);
                return paymentOrder;
            }
        }

        public async Task<PaymentOrder> Initiate(decimal amount, CurrencyEnum currency, int userId, BookmakerEnum bookmakerId, string ip)
        {
            if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";
            PaymentTransaction paymentTransaction = null;

            //Reserve the funds from the sport wallet
            var accountType = 1;
            var moduleId = 1;

            if (amount > 0)
            {
                paymentTransaction = await _paymentClient.InitiateDepositAsync(new DepositRequest()
                {
                    Amount = Math.Abs(amount),
                    ProviderId = (int)ProviderEnum.MstIppica
                },
                accountType,
                moduleId,
                ip,
                userId,
                null,
                (short)bookmakerId,
                userId,
                (short)bookmakerId);
            }
            else if (amount < 0)
            {
                paymentTransaction = await _paymentClient.RequestWithdrawalAsync(new WithdrawalRequest()
                {
                    Amount = Math.Abs(amount),
                    ProviderId = (int)ProviderEnum.MstIppica,
                    Description = "IPPICA",
                    Notes = "IPPICA"
                },
                    accountType,
                    (byte)1,
                    ip,
                    userId,
                    null,
                    (short)bookmakerId,
                    userId,
                    (short)bookmakerId
                );
            }

            if (paymentTransaction == null)
                throw new PaymentException(ReturnCodeEnum.TransactionNotCreated, "Financial transaction not created!");

            var paymentOrder = GetPaymentOrder(paymentTransaction.CorrelationTransactionId);
            paymentOrder.ThrowIf(x => x.StatusId != PaymentOrderStatusEnum.ToBeProcessed, new PaymentException(ReturnCodeEnum.TransactionNotCreated, "Transaction not initieted"));

            return paymentOrder;
        }

        public async Task<PaymentOrder> Refund(long paymentOrderId, int userId, BookmakerEnum bookmakerId, string ip)
        {
            var unitOfWork = _serviceProvider.GetService(typeof(IIsbetsUnitOfWork)) as IIsbetsUnitOfWork;
            using (unitOfWork)
            {
                if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";

                long? refundTransactionId = null;

                //Get basic transaction details
                var paymentOrder = GetPaymentOrder(paymentOrderId);

                if (paymentOrder.Amount is null || paymentOrder.Amount == 0)
                {
                    throw new PaymentException(ReturnCodeEnum.Unknown, "Payment order amount can't be equal to zero.");
                }

                var isBetAdminUser = false; //TODO!!! api currently does not cover BetAdmin users. 
                var accountType = isBetAdminUser ? 2 : 1;

                if (paymentOrder != null && paymentOrder.Amount < 0) //Withdrawal refund
                {
                    if (paymentOrder.StatusId == PaymentOrderStatusEnum.ToBeProcessed)
                    {
                        //Cancel pending transaction
                        var cancelResult = await _paymentClient.CancelWithdrawalAsync(
                            (long)paymentOrder.TransactionId,
                            accountType,
                            (byte)1,
                            ip,
                            userId,
                            null,
                            (short)bookmakerId,
                            userId,
                            (short)bookmakerId
                        );

                        refundTransactionId = cancelResult.RefundTransactionId;
                    }
                    else if (paymentOrder.StatusId == PaymentOrderStatusEnum.Done)
                    {
                        //Refund already settled transaction
                        var refundResult = await _paymentClient.RefundWithdrawalAsync(
                                            (long)paymentOrder.TransactionId,
                                            accountType,
                                            (byte)1,
                                            ip,
                                            userId,
                                            null,
                                            (short)bookmakerId,
                                            userId,
                                            (short)bookmakerId
                                        );

                        refundTransactionId = refundResult.RefundTransactionId;
                    }
                }
                else if (paymentOrder != null && paymentOrder.Amount > 0) //deposit refund
                {
                    //Refund already settled transaction
                    var refundResult = await _paymentClient.RefundDepositAsync(
                                        (long)paymentOrder.TransactionId,
                                        accountType,
                                        (byte)1,
                                        ip,
                                        userId,
                                        null,
                                        (short)bookmakerId,
                                        userId,
                                        (short)bookmakerId
                                    );

                    refundTransactionId = refundResult.RefundTransactionId;
                }


                //Check the status of refunded payment order
                paymentOrder = GetPaymentOrder((long)paymentOrder.PaymentOrderId);

                paymentOrder.ThrowIf(x => !new HashSet<PaymentOrderStatusEnum>
                {
                    PaymentOrderStatusEnum.ManuallyReversed,
                    PaymentOrderStatusEnum.ManuallyRefused
                }.Contains((PaymentOrderStatusEnum)x.StatusId), new PaymentException(ReturnCodeEnum.TransactionSettlementFailed, "Transaction not refunded"));

                //check transaction
                var transaction = (refundTransactionId != null) ? unitOfWork.PaymentOrderRepository.GetTransaction((long)refundTransactionId) :
                                                                  unitOfWork.PaymentOrderRepository.GetTransactions((long)paymentOrder.PaymentOrderId)?
                                                                                                   .Find(x => x.RefundedTransactionId != null && x.RefundedTransactionId == paymentOrder.TransactionId);

                transaction.ThrowIf(x => x == null, new PaymentException(ReturnCodeEnum.TransactionSettlementFailed, "Transaction not refunded"));

                var result = new PaymentOrder()
                {
                    TransactionId = transaction.TransactionId,
                    Amount = transaction.Amount,
                    CurrencyId = transaction.CurrencyId
                };

                return result;
            }
        }

        public async Task<PaymentOrder> Settle(long paymentOrderId, string externalId, int userId, BookmakerEnum bookmakerId, string ip)
        {
            if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";
            var isBetAdminUser = false; //TODO!!! api currently does not cover BetAdmin users. 
            var accountType = isBetAdminUser ? 2 : 1;
            var paymentTransaction = await _paymentClient.ProcessPaymentAsync(new PaymentState()
            {
                CorrelationTransactionId = paymentOrderId,
                State = PaymentStatus.Succeeded,
                Description = "IPPICA",
                Notes = "IPPICA",
                ThirdPartyId = externalId
            },
                accountType,
                (byte)1,
                ip,
                userId,
                null,
                (short)bookmakerId,
                userId,
                (short)bookmakerId
            );

            var paymentOrder = GetPaymentOrder(paymentTransaction.CorrelationTransactionId);
            paymentOrder.ThrowIf(x => x.StatusId != PaymentOrderStatusEnum.Done, new PaymentException(ReturnCodeEnum.TransactionNotCreated, "Transaction not settled"));

            return paymentOrder;
        }

        public async Task<PaymentOrder> SettleStakeCompensation(long paymentOrderId, string externalId, int userId, BookmakerEnum bookmakerId, string ip)
        {
            if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";
            var isBetAdminUser = false; //TODO!!! api currently does not cover BetAdmin users. 
            var accountType = isBetAdminUser ? 2 : 1;
            var paymentTransaction = await _paymentClient.ProcessPaymentAsync(new PaymentState()
            {
                CorrelationTransactionId = paymentOrderId,
                State = PaymentStatus.Succeeded,
                Description = "IPPICA",
                Notes = "IPPICA",
                ThirdPartyId = externalId,
                TipiSezione = 11 //Versamento 2 - Type used for stake compensation in order to let payment api pick the right Causali
            },
                accountType,
                (byte)1,
                ip,
                userId,
                null,
                (short)bookmakerId,
                userId,
                (short)bookmakerId
            );

            var paymentOrder = GetPaymentOrder(paymentTransaction.CorrelationTransactionId); //PaymentOrderFactory.Create(paymentTransaction);
            paymentOrder.ThrowIf(x => x.StatusId != PaymentOrderStatusEnum.Done, new PaymentException(ReturnCodeEnum.TransactionNotCreated, "Transaction not settled"));
            return paymentOrder;
        }

        public async Task<BetTransaction> ProcessTransaction(BetTransaction transaction, int userId, BookmakerEnum bookmakerId, string ip, bool processAsPending = false)
        {
            var startTime = DateTime.Now;

            var unitOfWork = _serviceProvider.GetService(typeof(IMstUnitOfWork)) as IMstUnitOfWork;
            using (unitOfWork)
            {
                transaction.ThrowIf(x => string.IsNullOrEmpty(x?.CurrencyCode), new PaymentException(ReturnCodeEnum.CurrencyCodeMissing));
                var currencyId = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), transaction.CurrencyCode);
                PaymentOrder paymentOrder = null;

                //if the transaction contains PaymentOrderId, check if it should be settled
                if (transaction?.PaymentOrderId != null)
                {
                    paymentOrder = GetPaymentOrder((long)transaction.PaymentOrderId);
                    if (!processAsPending &&
                       (paymentOrder.StatusId == PaymentOrderStatusEnum.ToBeProcessed ||
                        paymentOrder.StatusId == PaymentOrderStatusEnum.Pending))
                    {
                        paymentOrder = await Settle((long)paymentOrder.PaymentOrderId, transaction.BetTransactionId?.ToString(), userId, bookmakerId, ip);
                    }
                }
                else //create new transaction
                {
                    if (transaction.BetTransactionTypeId == BetTransactionTypeEnum.Stake ||
                        transaction.BetTransactionTypeId == BetTransactionTypeEnum.TaxStake)
                    {
                        //initiate stake
                        paymentOrder = await Initiate((decimal)transaction.Amount, currencyId, userId, bookmakerId, ip);

                        //settle, unless if a transaction should stay as "pending"
                        if (!processAsPending)
                        {
                            paymentOrder = await Settle((long)paymentOrder.PaymentOrderId, transaction.BetTransactionId?.ToString(), userId, bookmakerId, ip);
                        }
                    }
                    else if (transaction.BetTransactionTypeId == BetTransactionTypeEnum.RefundStake ||
                             transaction.BetTransactionTypeId == BetTransactionTypeEnum.RefundTaxStake ||
                             transaction.BetTransactionTypeId == BetTransactionTypeEnum.RefundWin ||
                             transaction.BetTransactionTypeId == BetTransactionTypeEnum.RefundTaxWin ||
                             transaction.BetTransactionTypeId == BetTransactionTypeEnum.RefundStakeCompensation)
                    {
                        transaction.ThrowIf(x => x.RefundBetTransactionId == null, new PaymentException(ReturnCodeEnum.TransactionNotFound, "Transaction does not contain RefundBetTransactionId"));
                        var transactionToRefund = unitOfWork.BetRepository.GetBetTransaction((long)transaction.RefundBetTransactionId)
                                                                          .ThrowIf(x => (x.PaymentOrderId ?? 0) == 0, new PaymentException(ReturnCodeEnum.PaymentOrderNotFound));
                        //Initiate fake PaymentOrder. Currently, refund cannot be taken as pending so we have to fake
                        paymentOrder = new PaymentOrder()
                        {
                            Amount = transaction.Amount,
                            CurrencyId = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), transaction.CurrencyCode)
                        };

                        if (!processAsPending)
                        {
                            paymentOrder = await Refund((long)transactionToRefund.PaymentOrderId, userId, bookmakerId, ip);
                        }
                    }
                    else if (transaction.BetTransactionTypeId == BetTransactionTypeEnum.StakeCompensation)
                    {
                        paymentOrder = await Initiate((decimal)transaction.Amount, currencyId, userId, bookmakerId, ip);

                        if (!processAsPending)
                        {
                            paymentOrder = await SettleStakeCompensation((long)paymentOrder.PaymentOrderId, transaction.BetTransactionId?.ToString(), userId, bookmakerId, ip);
                        }

                    }
                    else if (transaction.BetTransactionTypeId == BetTransactionTypeEnum.Win)
                    {
                        paymentOrder = await Initiate((decimal)transaction.Amount, currencyId, userId, bookmakerId, ip);

                        if (!processAsPending)
                        {
                            paymentOrder = await Settle((long)paymentOrder.PaymentOrderId, transaction.BetTransactionId?.ToString(), userId, bookmakerId, ip);
                        }
                    }
                }

                paymentOrder.ThrowIf(x => x == null, new PaymentException(ReturnCodeEnum.TransactionNotCreated, "Transaction not created"));

                transaction.Amount = paymentOrder?.Amount;
                transaction.PaymentOrderId = transaction.PaymentOrderId ?? paymentOrder?.PaymentOrderId;
                if ((transaction.TransactionId == null || transaction.TransactionId == -1) && paymentOrder?.TransactionId != null)
                    transaction.TransactionId = paymentOrder?.TransactionId;

                //log every time method take too much time to process
                var endTime = DateTime.Now;
                if ((endTime - startTime).TotalSeconds > 1)
                {
                    _logger.LogInformation("{MethodName}: UserId [{UserId}] - BetRequestId [{BetRequestId}] - BetId [{BetId}] - BetTransactionTypeId [{BetTransactionTypeId}] - PaymentOrderId [{PaymentOrderId}] - Total duration [{TotalDuration} sec]",
                                           "ProcessTransaction",
                                           userId,
                                           transaction.BetRequestId,
                                           transaction.BetId,
                                           transaction.BetTransactionTypeId,
                                           transaction.PaymentOrderId,
                                           (endTime - startTime).TotalSeconds
                                           );
                }

                return transaction;
            }
        }


        public async Task<PaymentOrder> UpdateThirdPartyId(long paymentOrderId, string thirdPartyId, int userId, BookmakerEnum bookmakerId, string ip)
        {
            if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";

            var isBetAdminUser = false; //TODO!!! api currently does not cover BetAdmin users. 
            var accountType = isBetAdminUser ? 2 : 1;
            var paymentTransaction = await _paymentClient.UpdateTransactionThirdPartyIdAsync(new UpdateTransactionThirdPartyIdRequest()
            {
                CorrelationTransactionId = paymentOrderId,
                ThirdPartyTransactionId = thirdPartyId
            },
                accountType,
                (byte)1,
                ip,
                userId,
                null,
                (short)bookmakerId,
                userId,
                (short)bookmakerId
            );

            return PaymentOrderFactory.Create(paymentTransaction);
        }
    }
}
