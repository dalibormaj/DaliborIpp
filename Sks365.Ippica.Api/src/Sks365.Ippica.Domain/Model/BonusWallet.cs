using Sks365.Ippica.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sks365.Ippica.Domain.Model
{
    public class BonusWallet : BaseDomainModel
    {
        public int? UserId { get; set; }
        public int? CampaignId { get; set; }
        public BookmakerEnum? BookmakerId { get; set; }
        public decimal? Balance { get; set; }
        public byte? BonusStatusId { get; set; }
        public Currency Currency { get; set; }
    }
}
