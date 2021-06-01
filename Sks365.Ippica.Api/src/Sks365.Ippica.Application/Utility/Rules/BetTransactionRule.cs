using Sks365.Ippica.Application.Services.Abstraction;
using Sks365.Ippica.Application.Utility.EmailSender;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.DataAccess;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.Ippica.Domain.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sks365.Ippica.Application.Utility.Rules
{
    internal class BetTransactionRule : IBetTransactionRule
    {
        private readonly List<BetTransaction> _newValues;
        private List<BetTransaction> _lastTransactions;
        private Bet _currentBetDb;
        private List<BetTransaction> _result;
        private IServiceProvider _serviceProvider;
        private readonly LanguageEnum _errorLanguageId;

        public BetTransactionRule(IServiceProvider serviceProvider, long? betId, List<BetTransaction> newValues, LanguageEnum errorLanguageId)
        {
            _errorLanguageId = errorLanguageId;
            if ((betId ?? 0) > 0)
            {
                var unitOfWork = serviceProvider.GetService(typeof(IMstUnitOfWork)) as IMstUnitOfWork;
                using (unitOfWork)
                {
                    _currentBetDb = unitOfWork.BetRepository.GetBet((long)betId, getDetails: false);
                    _lastTransactions = unitOfWork.BetRepository.GetLastBetTransactions((long)betId);
                }
            }

            _newValues = newValues;
            _result = newValues?.ToList(); //Clone the list
            _serviceProvider = serviceProvider;
        }

        public BetTransactionRule(IServiceProvider serviceProvider, long? betId, List<BetTransaction> newValues) : this(serviceProvider, betId, newValues, LanguageEnum.English)
        {
        }

        public List<BetTransaction> Execute()
        {
            PrepareRefund();
            PrepareAlreadyCreatedTransactions();
            ExecBaseValidations();

            return _result;
        }


        public IBetTransactionRule UniqueTypes()
        {
            if (_result?.Select(x => x.BetTransactionTypeId).Distinct().Count() != _result?.Count)
            {
                throw new IppicaException(ReturnCodeEnum.TransactionListMustContainUniqueValues, _errorLanguageId);
            }

            return this;
        }

        public IBetTransactionRule SupportedTypes(params BetTransactionTypeEnum[] types)
        {
            if (_result?.Exists(x => !types.ToList().Contains((BetTransactionTypeEnum)x.BetTransactionTypeId)) ?? false)
            {
                throw new IppicaException(ReturnCodeEnum.TransactionNotSupported, _errorLanguageId);
            }

            return this;
        }

        public IBetTransactionRule MustContain(params BetTransactionTypeEnum[] types)
        {
            if (!_result?.Exists(x => types.ToList().Contains((BetTransactionTypeEnum)x.BetTransactionTypeId)) ?? false)
            {
                var expected = string.Join(", ", types.ToList().ConvertAll(x => x.ToString()));
                throw new IppicaException(ReturnCodeEnum.TransactionNotFound, $"Transaction cannot be found. Expected: [{expected}]");
            }

            return this;
        }

        public IBetTransactionRule MustContainOneOf(params BetTransactionTypeEnum[] types)
        {
            if (!_result?.Any(x => types.ToList().Contains((BetTransactionTypeEnum)x.BetTransactionTypeId)) ?? false)
            {
                var expected = string.Join(" or ", types.ToList().ConvertAll(x => x.ToString()));
                throw new IppicaException(ReturnCodeEnum.TransactionNotFound, $"Transaction cannot be found. Expected: [{expected}]");
            }

            return this;
        }

        public IBetTransactionRule SameAsPreviousIf(params BetStatusEnum[] currentBetStatuses)
        {
            if ((_currentBetDb?.BetStatusId != null && currentBetStatuses != null &&
                 currentBetStatuses.ToList().Contains((BetStatusEnum)_currentBetDb.BetStatusId)) ||
                 currentBetStatuses == null)
            {
                if ((_lastTransactions?.Count ?? 0) != (_newValues?.Count ?? 0))
                    throw new IppicaException(ReturnCodeEnum.TransactionAmountDiffers, _errorLanguageId);

                foreach (var tran in _lastTransactions)
                {
                    if (!_newValues?.Exists(r => r.BetTransactionTypeId == tran.BetTransactionTypeId &&
                                                 r.Amount == tran.Amount) ?? false)
                    {
                        throw new IppicaException(ReturnCodeEnum.TransactionAmountDiffers, _errorLanguageId);
                    }
                };

                _result = _lastTransactions;
            }

            return this;
        }


        public IBetTransactionRule SameAsPrevious()
        {
            return SameAsPreviousIf(null);
        }

        //Refund previously created transactions but only if the bet is in one of the selected statuses
        public IBetTransactionRule RefundPreviousIf(params BetStatusEnum[] currentBetStatuses)
        {
            if ((_currentBetDb?.BetStatusId != null && currentBetStatuses != null &&
                 currentBetStatuses.ToList().Contains((BetStatusEnum)_currentBetDb.BetStatusId)) ||
                 currentBetStatuses == null)
            {
                var unitOfWork = _serviceProvider.GetService(typeof(IMstUnitOfWork)) as IMstUnitOfWork;
                using (unitOfWork)
                {
                    var lastTransactionsRefunded = unitOfWork.BetRepository.GetLastBetTransactions((long)_currentBetDb.BetId, convertToRefund: true);

                    if ((lastTransactionsRefunded?.Count ?? 0) == 0)
                        throw new IppicaException(ReturnCodeEnum.TransactionAmountDiffers, _errorLanguageId);

                    foreach (var tran in lastTransactionsRefunded)
                    {
                        //amounts and types must be the same
                        if ((_newValues?.Count() ?? 0) != 0 &&
                           (!_newValues.Exists(r => r.BetTransactionTypeId == tran.BetTransactionTypeId &&
                                                    r.Amount == tran.Amount)))
                        {
                            throw new IppicaException(ReturnCodeEnum.TransactionAmountDiffers, _errorLanguageId);
                        }
                    };

                    _result = lastTransactionsRefunded;
                }
            }

            return this;
        }

        //Validate newValues. The content must be the same as previously created transactions.
        //If there is no newValues the system will take the last created transaction and convert it to
        //refund types
        public IBetTransactionRule RefundPrevious()
        {
            return RefundPreviousIf(null);
        }

        public IBetTransactionRule CurrentStatus(params BetStatusEnum?[] statuses)
        {
            if (statuses != null && !statuses.ToList().Contains(_currentBetDb?.BetStatusId))
            {
                var expected = string.Join(", ", statuses.ToList().ConvertAll(x => (x == null) ? "null" : x.ToString()));
                var currentBetStatus = (_currentBetDb?.BetStatusId == null) ? "null" : ((BetStatusEnum)_currentBetDb.BetStatusId).ToString();
                throw new IppicaException(ReturnCodeEnum.BetInvalid, $"Bet cannot be processed. Current bet status: [{currentBetStatus}] - Expected: [{expected}]");
            }
            if (_currentBetDb?.BetStatusId == BetStatusEnum.Error && !statuses.ToList().Contains(BetStatusEnum.Error))
                throw new IppicaException(ReturnCodeEnum.Error, _errorLanguageId);

            return this;
        }


        public IBetTransactionRule SpecialCondition(Action condition)
        {
            condition.Invoke();
            return this;
        }

        /// <summary>
        /// Check if the user has enough funds to complete the action
        /// </summary>
        /// <param name="userId">User to be validated</param>
        /// <param name="email">Send an email in case validation fails</param>
        /// <returns></returns>
        public IBetTransactionRule CheckSufficientFunds(int? userId = null, Email email = null)
        {
            try
            {
                _currentBetDb.ThrowIf(x => x != null && userId != null && x.UserId != userId,
                                           new IppicaException(ReturnCodeEnum.Unknown, $"UserId is not equal to the user who owns the Bet (Id: {_currentBetDb?.BetId})"));

                userId = _currentBetDb?.UserId ?? userId;
                if (userId == null)
                    throw new IppicaException(ReturnCodeEnum.UserNotFound, _errorLanguageId);

                var totalAmount = _result.Sum(x => x.Amount);
                var sportWallet = (_serviceProvider.GetService(typeof(IUserService)) as IUserService)
                                                   .GetSportWallet((int)userId);
                if (sportWallet.Balance + totalAmount <= -0.01m)
                {
                    throw new IppicaException(ReturnCodeEnum.InsufficientFunds, _errorLanguageId);
                }
            }
            catch (IppicaException ex)
            {
                if (email != null && ex.ReturnCode == ReturnCodeEnum.InsufficientFunds)
                {
                    var emailSender = _serviceProvider.GetService(typeof(IEmailSender)) as IEmailSender;
                    emailSender.SendEmail(email, EmailRepetitionEnum.OnePerDay);
                }

                throw;
            }

            return this;
        }

        private void PrepareRefund()
        {
            //check if there is any valid transaction to refund
            using (var unitOfWork = _serviceProvider.GetService(typeof(IMstUnitOfWork)) as IMstUnitOfWork)
            {
                if (_result != null)
                {
                    foreach (var transaction in _result)
                    {
                        switch ((BetTransactionTypeEnum)transaction.BetTransactionTypeId)
                        {
                            case BetTransactionTypeEnum.RefundStake:
                                var refundStake = unitOfWork.BetRepository.GetActiveTransaction((long)_currentBetDb.BetId, BetTransactionTypeEnum.Stake, convertToRefund: true)
                                                                          .ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.StakeNotFound, _errorLanguageId))
                                                                          .ThrowIf(x => Math.Abs(transaction?.Amount ?? x?.Amount ?? 0) > Math.Abs(x?.Amount ?? 0),
                                                                                        new IppicaException(ReturnCodeEnum.RefundAmountCannotBeLargerThanStake, _errorLanguageId))
                                                                          .ThrowIf(x => Math.Abs(transaction?.Amount ?? x?.Amount ?? 0) != Math.Abs(x?.Amount ?? 0),
                                                                                        new IppicaException(ReturnCodeEnum.TransactionAmountDiffers, _errorLanguageId));

                                transaction.ApplyNewValues(refundStake); //alter transaction
                                break;
                            case BetTransactionTypeEnum.RefundStakeCompensation:
                                var refundStakeCompensation = unitOfWork.BetRepository.GetActiveTransaction((long)_currentBetDb.BetId, BetTransactionTypeEnum.StakeCompensation, convertToRefund: true)
                                                                                      .ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.StakeCompensationNotFound, _errorLanguageId))
                                                                                      .ThrowIf(x => Math.Abs(transaction?.Amount ?? x?.Amount ?? 0) != Math.Abs(x?.Amount ?? 0),
                                                                                                    new IppicaException(ReturnCodeEnum.TransactionAmountDiffers, _errorLanguageId));
                                transaction.ApplyNewValues(refundStakeCompensation); //alter transaction
                                break;
                            case BetTransactionTypeEnum.RefundWin:
                                var refundWin = unitOfWork.BetRepository.GetActiveTransaction((long)_currentBetDb.BetId, BetTransactionTypeEnum.Win, convertToRefund: true)
                                                                        .ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.WinNotFound, _errorLanguageId))
                                                                        .ThrowIf(x => Math.Abs(transaction?.Amount ?? x?.Amount ?? 0) != Math.Abs(x?.Amount ?? 0),
                                                                                      new IppicaException(ReturnCodeEnum.TransactionAmountDiffers, _errorLanguageId));
                                transaction.ApplyNewValues(refundWin); //alter transaction
                                break;

                            case BetTransactionTypeEnum.RefundTaxStake:
                                var refundTaxStake = unitOfWork.BetRepository.GetActiveTransaction((long)_currentBetDb.BetId, BetTransactionTypeEnum.TaxStake, convertToRefund: true)
                                                                             .ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.TaxStakeNotFound, _errorLanguageId))
                                                                             .ThrowIf(x => Math.Abs(transaction?.Amount ?? x?.Amount ?? 0) != Math.Abs(x?.Amount ?? 0),
                                                                                             new IppicaException(ReturnCodeEnum.TransactionAmountDiffers, _errorLanguageId));
                                transaction.ApplyNewValues(refundTaxStake); //alter transaction
                                break;

                            case BetTransactionTypeEnum.RefundTaxWin:
                                var refundTaxWin = unitOfWork.BetRepository.GetActiveTransaction((long)_currentBetDb.BetId, BetTransactionTypeEnum.TaxWin, convertToRefund: true)
                                                                           .ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.TaxWinNotFound, _errorLanguageId))
                                                                           .ThrowIf(x => Math.Abs(transaction?.Amount ?? x?.Amount ?? 0) != Math.Abs(x?.Amount ?? 0),
                                                                                         new IppicaException(ReturnCodeEnum.TransactionAmountDiffers, _errorLanguageId));
                                transaction.ApplyNewValues(refundTaxWin); //alter transaction
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void ExecBaseValidations()
        {
            if (_result != null)
            {

                var unitOfWork = _serviceProvider.GetService(typeof(IMstUnitOfWork)) as IMstUnitOfWork;
                using (unitOfWork)
                {
                    _result.Where(x => x.BetTransactionTypeId == BetTransactionTypeEnum.Stake)
                           .ToList()
                           .ForEach(x =>
                           {
                               if (x.Amount >= 0)
                                   throw new IppicaException(ReturnCodeEnum.StakeCannotBePositive, _errorLanguageId);
                           });

                    _result.Where(x => x.BetTransactionTypeId == BetTransactionTypeEnum.Win)
                           .ToList()
                           .ForEach(x =>
                           {
                               if (x.Amount < 0)
                                   throw new IppicaException(ReturnCodeEnum.WinCannotBeNegative, _errorLanguageId);
                               if (_currentBetDb != null && x.Amount > _currentBetDb.MaxWinning)
                                   throw new IppicaException(ReturnCodeEnum.BetInvalid, $"Win amount cannot be larger than MaxWinning: {_currentBetDb.MaxWinning}");
                           });

                    _result.Where(x => x.BetTransactionTypeId == BetTransactionTypeEnum.StakeCompensation)
                           .ToList()
                           .ForEach(x =>
                           {
                               if (x.Amount < 0)
                                   throw new IppicaException(ReturnCodeEnum.StakeCompensationCannotBeNegative, _errorLanguageId);

                               unitOfWork.BetRepository.GetActiveTransaction((long)_currentBetDb.BetId, BetTransactionTypeEnum.Stake)
                                                       .ThrowIf(stake => Math.Abs(x?.Amount ?? 0) >= Math.Abs(stake?.Amount ?? 0),
                                                                         new IppicaException(ReturnCodeEnum.RefundAmountCannotBeLargerOrEqualToStake, _errorLanguageId))
                                                       .ThrowIf(stake => _currentBetDb != null &&
                                                                          Math.Abs(x?.Amount ?? 0) > 0 && Math.Abs(x?.Amount ?? 0) < Math.Abs(stake?.Amount ?? 0) &&
                                                                         !(_currentBetDb.BetTypeId == BetTypeEnum.PsipTote || _currentBetDb.BetTypeId == BetTypeEnum.PsrTote),
                                                                         new IppicaException(ReturnCodeEnum.PartialRefundCanBeDoneForToteOnly, _errorLanguageId));
                           });
                }
            }
        }

        private void PrepareAlreadyCreatedTransactions()
        {
            //in case the new transaction is completely the same as the transaction created in the previous request
            //copy these values since it's the same transaction and it should not be created twice. This behaviour is
            //used to skip already created transaction when previous request has failed due to some error (for instance
            //if just 1 of 2 transactions is created, this transaction wont be created again)
            _result?.ForEach(x =>
            {
                var tran = _lastTransactions?.FirstOrDefault(s => s.BetTransactionTypeId == x.BetTransactionTypeId && s.Amount == x.Amount && s.CurrencyCode == x.CurrencyCode);
                if (tran != null)
                {
                    x.BetTransactionId = tran.BetTransactionId;
                    x.BetRequestId = tran.BetRequestId;
                    x.TransactionId = (x.TransactionId == null && tran?.TransactionId != null) ? tran?.TransactionId : x.TransactionId;
                    x.PaymentOrderId = (x.PaymentOrderId == null && tran?.PaymentOrderId != null) ? tran?.PaymentOrderId : x.PaymentOrderId;
                }

            });
        }
    }
}
