namespace Sks365.Ippica.Common.Utility
{
    public enum ReturnCodeEnum : int
    {
        Unknown = -1,
        Unauthorized = 1,
        Forbidden = 2,
        BadRequest = 3,
        SessionNotValid = 4,
        Error = 5,

        UserNotFound = 10,
        InvalidUser = 11,
        InvalidUserBalance = 12,

        //Bet result codes
        BetNotFound = 100,
        BetInvalid = 101,
        BetCannotBeCanceledWinExists = 102,
        BetCannotBeProcessedWrongType = 103,
        TicketDoesNotBelongToUser = 104,
        BetProcessedDataMismatched = 105,
        BetCannotBeRolledBack = 106,
        InvalidBetType = 107,
        ExternalIdAlreadyUsedByAnotherBet = 108,
        AnotherBetOperationStillRunning = 109,
        RequestNotAllowedForSelectedBookmaker = 110,
        OperationCannotComplete_SettlementIdMismatched = 111,

        //Transaction result codes
        StakeNotFound = 201,
        WinNotFound = 202,
        TransactionListMustContainUniqueValues = 203,
        StakeCompensationNotFound = 204,

        TransactionNotFound = 210,
        TransactionSettlementFailed = 211,
        TransactionNotCreated = 212,
        TransactionNotSupported = 213,
        StakeCannotBePositive = 214,
        PartialRefundCanBeDoneForToteOnly = 215,
        RefundAmountCannotBeLargerThanStake = 216,
        WinCannotBeNegative = 217,
        InsufficientFunds = 218,
        TransactionAmountDiffers = 219,
        RefundStakeNotFound = 220,
        CannotBePaid = 221,
        PaymentOrderNotFound = 222,
        CurrencyCodeMissing = 223,
        WinAlreadyProcessed = 224,
        TaxStakeNotFound = 225,
        TaxWinNotFound = 226,
        StakeCompensationCannotBeNegative = 227,
        RefundStakeCompensationNotFound = 228,
        RefundTaxStakeNotFound = 229,
        RefundTaxWinNotFound = 230,
        RefundWinNotFound = 231,
        RefundAmountCannotBeLargerOrEqualToStake = 232,
        TransactionExistsButInIrregularState = 233,

        OK = 1024,
        OK_AlreadyProcessed = 1025
    }
}
