using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model;

namespace Sks365.Ippica.Application.Utility.Rules
{
    internal class BetRuleResult
    {
        public Bet Bet { get; }
        public LanguageEnum UserLanguageId { get; }
        public bool IsRetry { get; }

        public BetRuleResult(Bet bet, bool isRetry)
        {
            Bet = bet;
            IsRetry = isRetry;
        }
    }
}