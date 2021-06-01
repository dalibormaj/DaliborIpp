using Sks365.Ippica.Common.Utility;
using System;
using System.Collections.Generic;

namespace Sks365.Ippica.Common.Exceptions
{
    public static class ReturnCodeTranslator
    {
        private static Dictionary<ReturnCodeEnum, string> EngTranslations => new Dictionary<ReturnCodeEnum, string>()
        {
            { ReturnCodeEnum.Unauthorized, "Unauthorized" },
            { ReturnCodeEnum.Forbidden, "Option not allowed" },
            { ReturnCodeEnum.BadRequest, "Bad request" },
            { ReturnCodeEnum.SessionNotValid, "Session not valid" },
            { ReturnCodeEnum.UserNotFound, "User not found" },
            { ReturnCodeEnum.InvalidUser, "Invalid user" },

            { ReturnCodeEnum.BetNotFound, "Bet cannot be found" },
            { ReturnCodeEnum.BetInvalid, "Bet invalid" },
            { ReturnCodeEnum.BetCannotBeCanceledWinExists, "Bet cannot be canceled. Please cancel the winning amount before cancelation" },
            { ReturnCodeEnum.BetCannotBeProcessedWrongType, "Bet cannot be processed due to wrong type" },

            { ReturnCodeEnum.TicketDoesNotBelongToUser, "The ticket does not belong to the selected user" },
            { ReturnCodeEnum.BetProcessedDataMismatched, "Bet is processed but some data mismatched" },
            { ReturnCodeEnum.BetCannotBeRolledBack, "Bet cannot be rolled back" },
            { ReturnCodeEnum.InvalidBetType, "Invalid bet type" },
            { ReturnCodeEnum.ExternalIdAlreadyUsedByAnotherBet, "ExternalId already used by another bet" },
            { ReturnCodeEnum.AnotherBetOperationStillRunning, "Another bet operation still running. Please try later"},
            { ReturnCodeEnum.RequestNotAllowedForSelectedBookmaker, "Request not allowed for selected bookmaker"},
            { ReturnCodeEnum.OperationCannotComplete_SettlementIdMismatched, "Operation cannot complete! SettlementId mismatched"},

            { ReturnCodeEnum.StakeNotFound, "Stake cannot be found" },
            { ReturnCodeEnum.WinNotFound, "Win cannot be found" },
            { ReturnCodeEnum.TransactionListMustContainUniqueValues, "Transaction list contains invalid values. All transactions should have unique type, duplicates are not allowed" },
            { ReturnCodeEnum.StakeCompensationNotFound, "Stake compensation cannot be found" },

            { ReturnCodeEnum.TransactionNotFound, "Transaction cannot be found" },
            { ReturnCodeEnum.TransactionSettlementFailed, "Transaction settlement failed" },
            { ReturnCodeEnum.TransactionNotCreated, "Transaction not created" },
            { ReturnCodeEnum.TransactionNotSupported, "Transaction not supported" },
            { ReturnCodeEnum.StakeCannotBePositive, "Stake cannot have positive value" },
            { ReturnCodeEnum.PartialRefundCanBeDoneForToteOnly, "Partial refund can be done for Tote game only" },
            { ReturnCodeEnum.RefundAmountCannotBeLargerThanStake, "Refund amount cannot be larger than stake" },
            { ReturnCodeEnum.WinCannotBeNegative, "Win transactions cannot be negative or zero" },
            { ReturnCodeEnum.InsufficientFunds, "Insufficient funds" },
            { ReturnCodeEnum.TransactionAmountDiffers, "Amount differs from previous request" },
            { ReturnCodeEnum.RefundStakeNotFound, "Refund stake cannot be found" },
            { ReturnCodeEnum.WinAlreadyProcessed, "Win already processed" },
            { ReturnCodeEnum.RefundAmountCannotBeLargerOrEqualToStake, "Refund amount cannot be larger or equal to stake" },
            { ReturnCodeEnum.TransactionExistsButInIrregularState, "Transaction exists but in irregular state" },
            { ReturnCodeEnum.OK, "OK" }
        };

        private static Dictionary<ReturnCodeEnum, string> ItaTranslations => new Dictionary<ReturnCodeEnum, string>()
        {
            { ReturnCodeEnum.Unauthorized, "Non autorizzato" },
            { ReturnCodeEnum.Forbidden, "Opzione non consentita" },
            { ReturnCodeEnum.BadRequest, "Brutta richiesta" },
            { ReturnCodeEnum.SessionNotValid, "Sessione non valida" },
            { ReturnCodeEnum.UserNotFound, "User not found" },
            { ReturnCodeEnum.InvalidUser, "Invalid user" },

            { ReturnCodeEnum.BetNotFound, "Bet cannot be found" },
            { ReturnCodeEnum.BetInvalid, "Bet invalid" },
            { ReturnCodeEnum.BetCannotBeCanceledWinExists, "Bet cannot be canceled. Please cancel the winning amount before cancelation" },
            { ReturnCodeEnum.BetCannotBeProcessedWrongType, "Bet cannot be processed due to wrong type" },

            { ReturnCodeEnum.TicketDoesNotBelongToUser, "The ticket does not belong to the selected user" },
            { ReturnCodeEnum.BetProcessedDataMismatched, "Bet is processed but some data mismatched" },
            { ReturnCodeEnum.BetCannotBeRolledBack, "Bet cannot be rolled back" },
            { ReturnCodeEnum.InvalidBetType, "Invalid bet type" },
            { ReturnCodeEnum.ExternalIdAlreadyUsedByAnotherBet, "ExternalId already used by another bet!" },

            { ReturnCodeEnum.StakeNotFound, "Stake cannot be found" },
            { ReturnCodeEnum.WinNotFound, "La vincita non puo' essere trovata." },
            { ReturnCodeEnum.TransactionListMustContainUniqueValues, "Transaction list contains invalid values. All transactions should have unique type, duplicates are not allowed" },
            { ReturnCodeEnum.StakeCompensationNotFound, "Stake compensation cannot be found. Check refund amount" },

            { ReturnCodeEnum.TransactionNotFound, "Transaction cannot be found" },
            { ReturnCodeEnum.TransactionSettlementFailed, "Transaction settlement failed" },
            { ReturnCodeEnum.TransactionNotCreated, "Transaction not created" },
            { ReturnCodeEnum.TransactionNotSupported, "Transazione non supportata" },
            { ReturnCodeEnum.StakeCannotBePositive, "Stake cannot have positive value" },
            { ReturnCodeEnum.PartialRefundCanBeDoneForToteOnly, "Partial refund can be done for Tote game only" },
            { ReturnCodeEnum.RefundAmountCannotBeLargerThanStake, "L'importo rimborsato non puo' essere superiore all'importo giocato." },
            { ReturnCodeEnum.WinCannotBeNegative, "Win transactions cannot be negative or zero" },
            { ReturnCodeEnum.InsufficientFunds, "Fondi insufficienti" },
            { ReturnCodeEnum.TransactionAmountDiffers, "Amount differs from previous request" },
            { ReturnCodeEnum.RefundStakeNotFound, "Refund stake cannot be found" },
            { ReturnCodeEnum.WinAlreadyProcessed, "Win already processed" },
            { ReturnCodeEnum.RefundAmountCannotBeLargerOrEqualToStake, "L'importo rimborsato non puo' essere uguale o superiore all'importo giocato" },
            { ReturnCodeEnum.OK, "OK" }
        };

        public static string Translate(ReturnCodeEnum? returnCode, LanguageEnum? language)
        {
            language = language ?? LanguageEnum.English; //Default API translator language
            var transaction = string.Empty;

            if (returnCode.HasValue)
            {
                if (language == LanguageEnum.Italian)
                {
                    ItaTranslations.TryGetValue((ReturnCodeEnum)returnCode, out transaction);
                }

                if (string.IsNullOrEmpty(transaction))
                {
                    EngTranslations.TryGetValue((ReturnCodeEnum)returnCode, out transaction);
                }

                //If no translations have found use the Enum name
                if (string.IsNullOrEmpty(transaction))
                    transaction = Enum.GetName(typeof(ReturnCodeEnum), returnCode);
            }

            return transaction;
        }
    }
}
