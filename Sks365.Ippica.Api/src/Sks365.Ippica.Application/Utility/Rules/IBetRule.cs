using Sks365.Ippica.Domain.Model.Enums;
using System;

namespace Sks365.Ippica.Application.Utility.Rules
{
    /// <summary>
    /// Validate the Bet and get the savable values
    /// </summary>
    internal interface IBetRule
    {
        BetRuleResult Execute();
        IBetRule CurrentStatus(params BetStatusEnum?[] statuses);
        IBetRule NewStatus(BetStatusEnum targetBetStatus);
        IBetRule SpecialCondition(Action condition);
    }
}
