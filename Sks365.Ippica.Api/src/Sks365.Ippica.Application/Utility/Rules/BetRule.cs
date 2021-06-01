using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.DataAccess;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.Ippica.Domain.Utility;
using System;
using System.Linq;

namespace Sks365.Ippica.Application.Utility.Rules
{
    internal class BetRule : IBetRule
    {
        private Bet _currentBetDb;
        private Bet _newBet;
        private BetStatusEnum? _targetBetStatus = null;
        private readonly LanguageEnum _errorLanguageId;
        private readonly BetRequest _newBetRequest;
        private readonly IServiceProvider _serviceProvider;

        public BetRule(IServiceProvider serviceProvider, BetRequest newBetRequest, LanguageEnum errorLanguageId)
        {
            _serviceProvider = serviceProvider;
            _newBetRequest = newBetRequest;
            _errorLanguageId = errorLanguageId;

            var unitOfWork = _serviceProvider.GetService(typeof(IMstUnitOfWork)) as IMstUnitOfWork;
            using (unitOfWork)
            {
                if (!string.IsNullOrEmpty(_newBetRequest.ExternalId))
                    _currentBetDb = unitOfWork.BetRepository.GetBetByExternalId(_newBetRequest.ExternalId);

                if (!string.IsNullOrEmpty(_newBetRequest.TicketId) && _currentBetDb == null)
                    _currentBetDb = unitOfWork.BetRepository.GetBet(_newBetRequest.TicketId);
            }
        }

        public BetRule(IServiceProvider serviceProvider, BetRequest newBetRequest) : this(serviceProvider, newBetRequest, LanguageEnum.English)
        {
        }

        public BetRule(IServiceProvider serviceProvider, BetRequest newBetRequest, Bet newBet, LanguageEnum errorLanguageId) : this(serviceProvider, newBetRequest, errorLanguageId)
        {
            _newBet = newBet;
        }

        public BetRule(IServiceProvider serviceProvider, BetRequest newBetRequest, Bet newBet) : this(serviceProvider, newBetRequest, newBet, LanguageEnum.English)
        {
        }

        public BetRuleResult Execute()
        {
            ExecBaseValidations();

            _targetBetStatus = _targetBetStatus ?? _currentBetDb?.BetStatusId;
            var isRetry = _currentBetDb?.BetStatusId == _targetBetStatus ||
                          (_currentBetDb?.BetStatusId == BetStatusEnum.Won && _targetBetStatus == BetStatusEnum.WonNotPaid) || //edge case. It can happen when MST sends PAY before SETTLE (Shop) so we should send them OK instead of rejection
                          (_currentBetDb?.BetStatusId == BetStatusEnum.Refunded && _targetBetStatus == BetStatusEnum.RefundedNotPaid); //edge case

            if (isRetry)
            {
                //If _betSettlementId is passed, in order to determine if this is a RETRY ATTEMPT, we have to compare the _betSettlementId too.
                //If it's not the same as ID sent in the last request throw an exception. Comparation should be done just for WebSettlement,
                //since the field does not exist in request for Shops. 
                if ((_currentBetDb?.UserId ?? 0) > 0 && (_newBetRequest?.WebBetRequest?.BetSettlementId ?? 0) > 0)
                {
                    var unitOfWork = _serviceProvider.GetService(typeof(IMstUnitOfWork)) as IMstUnitOfWork;
                    var lastBetRequest = unitOfWork.BetRepository.GetLastBetRequest((long)_currentBetDb.BetId);
                    var isWebSettle = lastBetRequest?.BetRequestTypeId == BetRequestTypeEnum.WebSettleBet;
                    var isReasonIdTheSame = (lastBetRequest?.WebBetRequest?.BetSettlementReasonId == _newBetRequest?.WebBetRequest?.BetSettlementReasonId);
                    var isSettlementIdTheSame = lastBetRequest?.BetRequestTypeId == _newBetRequest?.BetRequestTypeId &&
                                                lastBetRequest?.WebBetRequest?.BetSettlementReasonId == _newBetRequest?.WebBetRequest?.BetSettlementReasonId &&
                                                lastBetRequest?.WebBetRequest?.BetSettlementId == _newBetRequest?.WebBetRequest?.BetSettlementId;
                    if (isWebSettle && isReasonIdTheSame && !isSettlementIdTheSame)
                    {
                        throw new IppicaException(ReturnCodeEnum.OperationCannotComplete_SettlementIdMismatched);
                    }
                }
            }
            else
            {
                _currentBetDb = _currentBetDb.ApplyNewValues(_newBet);

                if (_currentBetDb != null)
                    _currentBetDb.BetStatusId = _targetBetStatus ?? _currentBetDb.BetStatusId; //set a new BetStatus
            }

            return new BetRuleResult(_currentBetDb, isRetry);
        }

        public IBetRule CurrentStatus(params BetStatusEnum?[] statuses)
        {
            if (statuses != null && !statuses.ToList().Contains(_currentBetDb?.BetStatusId))
            {
                var expected = string.Join(", ", statuses.Distinct().ToList().ConvertAll(x => (x == null) ? "null" : x.ToString()));
                var currentBetStatus = (_currentBetDb?.BetStatusId == null) ? "null" : ((BetStatusEnum)_currentBetDb.BetStatusId).ToString();
                throw new IppicaException(ReturnCodeEnum.BetInvalid, $"Bet cannot be processed. Current bet status: [{currentBetStatus}] - Expected: [{expected}]");
            }
            if (_currentBetDb?.BetStatusId == BetStatusEnum.Error && !statuses.ToList().Contains(BetStatusEnum.Error))
                throw new IppicaException(ReturnCodeEnum.Error, _errorLanguageId);

            return this;
        }

        public IBetRule NewStatus(BetStatusEnum targetBetStatus)
        {
            _targetBetStatus = targetBetStatus;
            return this;
        }

        public IBetRule SpecialCondition(Action condition)
        {
            condition.Invoke();
            return this;
        }

        private void ExecBaseValidations()
        {
            if (_newBet != null)
            {
                _currentBetDb.ThrowIf(x => x?.BetTypeId != null && _newBet?.BetTypeId != null && x.BetTypeId != _newBet.BetTypeId,
                                           new IppicaException(ReturnCodeEnum.BetCannotBeProcessedWrongType, _errorLanguageId))
                             .ThrowIf(x => !string.IsNullOrEmpty(_currentBetDb?.ExternalId) && (x?.TicketId ?? _newBet?.TicketId) != _newBet?.TicketId,
                                           new IppicaException(ReturnCodeEnum.ExternalIdAlreadyUsedByAnotherBet, _errorLanguageId));
            }
        }
    }
}
