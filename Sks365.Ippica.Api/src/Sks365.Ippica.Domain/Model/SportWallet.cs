using Sks365.Ippica.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sks365.Ippica.Domain.Model
{
    public class SportWallet : BaseDomainModel
    {
        public int? UserId { get; set; }
        public BookmakerEnum? BookmakerId { get; set; }
        public decimal? Balance { get; set; }
        public decimal? WithdrawableBalance { get; set; }
        public decimal? Reservation { get; set; }
        public decimal? Overdraft { get; set; }
        public Currency Currency { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }
    }
}
