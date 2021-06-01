namespace Sks365.Ippica.Domain.Model.Enums
{
    public enum BetTransactionTypeEnum : int
    {
        Stake = 1,
        StakeCompensation = 2,
        Win = 3,
        TaxStake = 4,
        TaxWin = 5,
        RefundStake = 11,
        RefundStakeCompensation = 12,
        RefundWin = 13,
        RefundTaxStake = 14,
        RefundTaxWin = 15
    }
}
