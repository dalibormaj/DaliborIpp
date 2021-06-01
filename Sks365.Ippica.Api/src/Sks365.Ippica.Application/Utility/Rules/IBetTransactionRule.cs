using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System;
using System.Collections.Generic;

namespace Sks365.Ippica.Application.Utility.Rules
{
    /// <summary>
    /// Validate the list of BetTransactions and get the savable values
    /// </summary>
    internal interface IBetTransactionRule
    {
        List<BetTransaction> Execute();
        IBetTransactionRule UniqueTypes();
        IBetTransactionRule SupportedTypes(params BetTransactionTypeEnum[] types);
        IBetTransactionRule MustContain(params BetTransactionTypeEnum[] types);
        IBetTransactionRule MustContainOneOf(params BetTransactionTypeEnum[] types);
        IBetTransactionRule SameAsPreviousIf(params BetStatusEnum[] currentBetStatuses);
        IBetTransactionRule SameAsPrevious();
        IBetTransactionRule RefundPreviousIf(params BetStatusEnum[] currentBetStatuses);
        IBetTransactionRule RefundPrevious();
        IBetTransactionRule CurrentStatus(params BetStatusEnum?[] statuses);
        IBetTransactionRule SpecialCondition(Action condition);
        IBetTransactionRule CheckSufficientFunds(int? userId = null, Email email = null);
    }
}
