using Sks365.Ippica.Domain.Model.Enums;
using System;
using System.Collections.Generic;

namespace Sks365.Ippica.Domain.Model
{
    [Serializable]
    public class Bet : BaseDomainModel
    {
        public long? BetId { get; set; }
        public BetStatusEnum? BetStatusId { get; set; }
        public BetTypeEnum? BetTypeId { get; set; }
        public BookmakerEnum? BookmakerId { get; set; }
        public int? UserId { get; set; }
        public string TicketId { get; set; }
        public decimal? Amount { get; set; } = 0;
        public decimal? Stake { get; set; } = 0;
        public CurrencyEnum? CurrencyId { get; set; }
        public decimal? TaxStake { get; set; } = 0;
        public decimal? WinAmount { get; set; } = 0;
        public decimal? TaxWin { get; set; } = 0;
        public string ExternalId { get; set; }
        public DateTime? Emission { get; set; }
        public DateTime? EmissionUtc { get; set; }
        public decimal? MaxWinning { get; set; }
        public int? Bets { get; set; } = 0;
        public DateTime? Competence { get; set; }
        public int? Type { get; set; }
        public string BonusId { get; set; }
        public decimal? Bonus { get; set; }
        public int? Source { get; set; }
        public int? Antepost { get; set; }
        public decimal? RefundAmount { get; set; } = 0;
        public List<FixBetDetail> FixBetDetails { get; set; }
        public List<SysBetDetail> SysBetDetails { get; set; }
        public PsipBetDetail PsipBetDetails { get; set; }
        public PsrBetDetail PsrBetDetails { get; set; }

        public Bet()
        {

        }

        public Bet(List<FixBetDetail> fixBetDetails)
        {
            FixBetDetails = fixBetDetails;
            BetTypeId = BetTypeEnum.Fix;
        }

        public Bet(List<SysBetDetail> sysBetDetails)
        {
            SysBetDetails = sysBetDetails;
            BetTypeId = BetTypeEnum.System;
        }

        public Bet(PsipBetDetail psipBetDetails)
        {
            PsipBetDetails = psipBetDetails;
            BetTypeId = BetTypeEnum.PsipTote;
        }

        public Bet(PsrBetDetail psrBetDetails)
        {
            PsrBetDetails = psrBetDetails;
            BetTypeId = BetTypeEnum.PsrTote;
        }
    }
}
