namespace Sks365.Ippica.Domain.Model.Enums
{
    public enum PaymentOrderStatusEnum
    {
        Done = 1,
        DoneWithErrors = 2,
        Pending = 3,
        ManuallyReversed = 4,
        ToBeProcessed = 5,
        ToBeVerified = 6,
        ManuallyRefused = 7
    }
}
