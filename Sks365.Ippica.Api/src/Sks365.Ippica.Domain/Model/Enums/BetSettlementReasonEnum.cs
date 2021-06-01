namespace Sks365.Ippica.Domain.Model.Enums
{
    public enum BetSettlementReasonEnum : int
    {
        Payment = 1,
        Refund = 2,
        CancelPayment = 3,
        CancelRefund = 4,
        Losers = 5,
        TicketReopened = 6,
        PaymentAndRefund = 7,
        CancelPaymentAndRefund = 8
    }
}
