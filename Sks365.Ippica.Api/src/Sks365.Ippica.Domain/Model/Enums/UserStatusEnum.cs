namespace Sks365.Ippica.Domain.Model.Enums
{
    public enum UserStatusEnum : byte
    {
        Enabled = 1,
        Disabled = 2,
        Deleted = 3,
        BeValidated = 4,
        ErrorCreatingUser = 6,
        ReadOnly = 7,
        PartiallyOpen = 9,
        PreClosed = 10,
        Dormant = 11,
        Preregistered = 12,
        WithdrawalOnly = 13
    }
}