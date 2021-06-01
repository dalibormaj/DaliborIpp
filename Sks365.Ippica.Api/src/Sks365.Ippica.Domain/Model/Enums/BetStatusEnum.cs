namespace Sks365.Ippica.Domain.Model.Enums
{
    public enum BetStatusEnum : int
    {
        Refunded = 10,
        RefundedNotPaid = 11,
        Reserved = 20,
        Placed = 30,
        Lost = 40,
        WonNotPaid = 41,
        Won = 45,
        Error = 100
    }
}
