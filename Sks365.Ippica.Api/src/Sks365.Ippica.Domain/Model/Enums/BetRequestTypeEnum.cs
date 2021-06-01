using System.ComponentModel;

namespace Sks365.Ippica.Domain.Model.Enums
{
    public enum BetRequestTypeEnum : int
    {
        [Description("Web")]
        WebReserveBet = 1,
        [Description("Web")]
        WebPlaceBet = 2,
        [Description("Web")]
        WebRollbackBet = 3,
        [Description("Web")]
        WebSettleBet = 4,

        [Description("Shop")]
        ShopReserveBet = 5,
        [Description("Shop")]
        ShopPlaceBet = 6,
        [Description("Shop")]
        ShopRollbackBet = 7,
        [Description("Shop")]
        ShopCancelBet = 8,
        [Description("Shop")]
        ShopSettleBet = 9,
        [Description("Shop")]
        PayBet = 10
    }
}
