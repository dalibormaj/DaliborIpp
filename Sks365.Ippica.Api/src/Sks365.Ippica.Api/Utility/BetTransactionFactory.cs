using Sks365.Ippica.Api.Dto;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System;
using System.Collections.Generic;

namespace Sks365.Ippica.Api.Utility
{
    public static class BetTransactionFactory
    {

        public static BetTransaction Create(BetTransactionTypeEnum type, decimal? amount, string currencyCode)
        {
            BetTransaction transaction = null;
            if (type == BetTransactionTypeEnum.Stake ||
                type == BetTransactionTypeEnum.TaxStake ||
                type == BetTransactionTypeEnum.RefundStakeCompensation ||
                type == BetTransactionTypeEnum.RefundWin ||
                type == BetTransactionTypeEnum.RefundTaxWin)
            {
                transaction = new BetTransaction()
                {
                    BetTransactionTypeId = type,
                    Amount = amount.HasValue ? Math.Abs((decimal)amount) * -1 : amount,
                    CurrencyCode = currencyCode
                };
            }
            else if (type == BetTransactionTypeEnum.Win ||
                     type == BetTransactionTypeEnum.TaxWin ||
                     type == BetTransactionTypeEnum.StakeCompensation ||
                     type == BetTransactionTypeEnum.RefundStake ||
                     type == BetTransactionTypeEnum.RefundTaxStake)
            {
                transaction = new BetTransaction()
                {
                    BetTransactionTypeId = type,
                    Amount = amount.HasValue ? Math.Abs((decimal)amount) : amount,
                    CurrencyCode = currencyCode.ToUpper()
                };
            }

            return transaction;
        }

        #region Web
        public static List<BetTransaction> Create(WebReserveBetRequest<BetDto> dto)
        {
            var result = new List<BetTransaction>();
            if (dto.Bet.Header.Stake > 0)
                result.Add(Create(BetTransactionTypeEnum.Stake, (decimal)dto.Bet.Header.Stake / 100, dto.Currency));

            if (dto.TaxStake > 0)
                result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));

            return result;
        }

        public static List<BetTransaction> Create(WebReserveBetRequest<PsipBetDto> dto)
        {
            var result = new List<BetTransaction>();
            if (dto.Bet.Prezzo > 0)
                result.Add(Create(BetTransactionTypeEnum.Stake, (decimal)dto.Bet.Prezzo / 100, dto.Currency));

            if (dto.TaxStake > 0)
                result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));

            return result;
        }

        public static List<BetTransaction> Create(WebReserveBetRequest<PsrBetDto> dto)
        {
            var result = new List<BetTransaction>();
            if (dto.Bet.Prezzo > 0)
                result.Add(Create(BetTransactionTypeEnum.Stake, (decimal)dto.Bet.Prezzo / 100, dto.Currency));

            if (dto.TaxStake > 0)
                result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));

            return result;
        }

        public static List<BetTransaction> Create(WebPlaceBetRequest<BetDto> dto)
        {
            var result = new List<BetTransaction>();
            if (dto.Bet.Header.Stake > 0)
                result.Add(Create(BetTransactionTypeEnum.Stake, (decimal)dto.Bet.Header.Stake / 100, dto.Currency));

            if (dto.TaxStake > 0)
                result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));

            return result;
        }

        public static List<BetTransaction> Create(WebPlaceBetRequest<PsipBetDto> dto)
        {
            var result = new List<BetTransaction>();
            if (dto.Bet.Prezzo > 0)
                result.Add(Create(BetTransactionTypeEnum.Stake, (decimal)dto.Bet.Prezzo / 100, dto.Currency));

            if (dto.TaxStake > 0)
                result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));

            return result;
        }

        public static List<BetTransaction> Create(WebPlaceBetRequest<PsrBetDto> dto)
        {
            var result = new List<BetTransaction>();
            if (dto.Bet.Prezzo > 0)
                result.Add(Create(BetTransactionTypeEnum.Stake, (decimal)dto.Bet.Prezzo / 100, dto.Currency));

            if (dto.TaxStake > 0)
                result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));

            return result;
        }


        public static List<BetTransaction> Create(WebRollbackBetRequest dto)
        {
            var result = new List<BetTransaction>();
            result.Add(Create(BetTransactionTypeEnum.RefundStake, null, null));
            result.Add(Create(BetTransactionTypeEnum.TaxStake, null, null));

            return result;
        }

        public static List<BetTransaction> Create(WebSettleBetRequest dto)
        {
            var result = new List<BetTransaction>();

            if (dto.PaymentAmount > 0)
            {
                switch (dto.Reason)
                {
                    case BetSettlementReasonEnum.Payment:
                        result.Add(Create(BetTransactionTypeEnum.Win, (decimal)dto.PaymentAmount / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.Refund:
                        break;
                    case BetSettlementReasonEnum.CancelPayment:
                        result.Add(Create(BetTransactionTypeEnum.RefundWin, (decimal)dto.PaymentAmount / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.CancelRefund:
                        break;
                    case BetSettlementReasonEnum.Losers:
                        break;
                    case BetSettlementReasonEnum.TicketReopened:
                        break;
                    case BetSettlementReasonEnum.PaymentAndRefund:
                        result.Add(Create(BetTransactionTypeEnum.Win, (decimal)dto.PaymentAmount / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.CancelPaymentAndRefund:
                        result.Add(Create(BetTransactionTypeEnum.RefundWin, (decimal)dto.PaymentAmount / 100, dto.Currency));
                        break;
                    default:
                        break;
                }
            }

            if (dto.RefundAmount > 0)
            {
                switch (dto.Reason)
                {
                    case BetSettlementReasonEnum.Payment:
                        break;
                    case BetSettlementReasonEnum.Refund:
                        result.Add(Create(BetTransactionTypeEnum.RefundStake, (decimal)dto.RefundAmount / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.CancelPayment:
                        break;
                    case BetSettlementReasonEnum.CancelRefund:
                        result.Add(Create(BetTransactionTypeEnum.Stake, (decimal)dto.RefundAmount / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.Losers:
                        break;
                    case BetSettlementReasonEnum.TicketReopened:
                        result.Add(Create(BetTransactionTypeEnum.RefundStakeCompensation, (decimal)dto.RefundAmount / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.PaymentAndRefund:
                        result.Add(Create(BetTransactionTypeEnum.StakeCompensation, (decimal)dto.RefundAmount / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.CancelPaymentAndRefund:
                        result.Add(Create(BetTransactionTypeEnum.RefundStakeCompensation, (decimal)dto.RefundAmount / 100, dto.Currency));
                        break;
                    default:
                        break;
                }
            }

            if (dto.TaxStake > 0)
            {
                switch (dto.Reason)
                {
                    case BetSettlementReasonEnum.Payment:
                        result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.Refund:
                        result.Add(Create(BetTransactionTypeEnum.RefundTaxStake, (decimal)dto.TaxStake / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.CancelPayment:
                        break;
                    case BetSettlementReasonEnum.CancelRefund:
                        result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.Losers:
                        result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.TicketReopened:
                        result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.PaymentAndRefund:
                        result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.CancelPaymentAndRefund:
                        result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));
                        break;
                    default:
                        break;
                }
            }

            if (dto.TaxWin > 0)
            {
                switch (dto.Reason)
                {
                    case BetSettlementReasonEnum.Payment:
                        result.Add(Create(BetTransactionTypeEnum.TaxWin, (decimal)dto.TaxWin / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.Refund:
                        break;
                    case BetSettlementReasonEnum.CancelPayment:
                        result.Add(Create(BetTransactionTypeEnum.RefundTaxWin, (decimal)dto.TaxWin / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.CancelRefund:
                        break;
                    case BetSettlementReasonEnum.Losers:
                        break;
                    case BetSettlementReasonEnum.TicketReopened:
                        break;
                    case BetSettlementReasonEnum.PaymentAndRefund:
                        result.Add(Create(BetTransactionTypeEnum.TaxWin, (decimal)dto.TaxWin / 100, dto.Currency));
                        break;
                    case BetSettlementReasonEnum.CancelPaymentAndRefund:
                        result.Add(Create(BetTransactionTypeEnum.RefundTaxWin, (decimal)dto.TaxWin / 100, dto.Currency));
                        break;
                    default:
                        break;
                }
            }

            return result;
        }
        #endregion


        #region Shop
        public static List<BetTransaction> Create(ShopReserveBetRequest<BetDto> dto)
        {
            var result = new List<BetTransaction>();
            if (dto.JBet.Header.Stake > 0)
                result.Add(Create(BetTransactionTypeEnum.Stake, (decimal)dto.JBet.Header.Stake / 100, dto.Currency));

            if (dto.TaxStake > 0)
                result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));

            return result;
        }

        public static List<BetTransaction> Create(ShopReserveBetRequest<PsipBetDto> dto)
        {
            var result = new List<BetTransaction>();
            if (dto.JBet.Prezzo > 0)
                result.Add(Create(BetTransactionTypeEnum.Stake, (decimal)dto.JBet.Prezzo / 100, dto.Currency));

            if (dto.TaxStake > 0)
                result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));

            return result;
        }

        public static List<BetTransaction> Create(ShopReserveBetRequest<PsrBetDto> dto)
        {
            var result = new List<BetTransaction>();
            if (dto.JBet.Prezzo > 0)
                result.Add(Create(BetTransactionTypeEnum.Stake, (decimal)dto.JBet.Prezzo / 100, dto.Currency));

            if (dto.TaxStake > 0)
                result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));

            return result;
        }

        public static List<BetTransaction> Create(ShopPlaceBetRequest<BetDto> dto)
        {
            var result = new List<BetTransaction>();
            if (dto.JBet.Header.Stake > 0)
                result.Add(Create(BetTransactionTypeEnum.Stake, (decimal)dto.JBet.Header.Stake / 100, dto.Currency));

            if (dto.TaxStake > 0)
                result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));

            return result;
        }

        public static List<BetTransaction> Create(ShopPlaceBetRequest<PsipBetDto> dto)
        {
            var result = new List<BetTransaction>();
            if (dto.JBet.Prezzo > 0)
                result.Add(Create(BetTransactionTypeEnum.Stake, (decimal)dto.JBet.Prezzo / 100, dto.Currency));

            if (dto.TaxStake > 0)
                result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));

            return result;
        }

        public static List<BetTransaction> Create(ShopPlaceBetRequest<PsrBetDto> dto)
        {
            var result = new List<BetTransaction>();
            if (dto.JBet.Prezzo > 0)
                result.Add(Create(BetTransactionTypeEnum.Stake, (decimal)dto.JBet.Prezzo / 100, dto.Currency));

            if (dto.TaxStake > 0)
                result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, dto.Currency));

            return result;
        }


        public static List<BetTransaction> Create(ShopRollbackBetRequest dto)
        {
            var result = new List<BetTransaction>();
            //result.Add(Create(BetTransactionTypeEnum.RefundStake, null, null));
            //result.Add(Create(BetTransactionTypeEnum.RefundTaxStake, null, null));

            return result;
        }

        public static List<BetTransaction> Create(ShopCancelBetRequest dto)
        {
            var result = new List<BetTransaction>();

            if (dto.Stake > 0)
                result.Add(Create(BetTransactionTypeEnum.RefundStake, (decimal)dto.Stake / 100, dto.Currency));

            if (dto.TaxStake > 0)
                result.Add(Create(BetTransactionTypeEnum.RefundTaxStake, (decimal)dto.TaxStake / 100, dto.Currency));

            return result;
        }

        public static List<BetTransaction> Create(TicketDto dto)
        {
            var result = new List<BetTransaction>();

            var eur = Enum.GetName(typeof(CurrencyEnum), CurrencyEnum.EUR);
            var status = (BetSettlementStatusEnum)Enum.Parse(typeof(BetSettlementStatusEnum), dto.Status);

            if (dto.WinAmount > 0)
            {
                switch (status)
                {
                    case BetSettlementStatusEnum.W:
                        result.Add(Create(BetTransactionTypeEnum.Win, (decimal)dto.WinAmount / 100, eur));
                        break;
                    case BetSettlementStatusEnum.V:
                        result.Add(Create(BetTransactionTypeEnum.RefundStake, (decimal)dto.RefundAmount / 100, eur));
                        break;
                    default:
                        break;
                }

            }

            if (dto.RefundAmount > 0)
            {
                switch (status)
                {
                    case BetSettlementStatusEnum.W:
                        result.Add(Create(BetTransactionTypeEnum.StakeCompensation, (decimal)dto.RefundAmount / 100, eur));
                        break;
                    case BetSettlementStatusEnum.V:
                        result.Add(Create(BetTransactionTypeEnum.RefundStake, (decimal)dto.RefundAmount / 100, eur));
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public static List<BetTransaction> Create(ShopPayBetRequest dto)
        {
            var result = new List<BetTransaction>();
            var eur = Enum.GetName(typeof(CurrencyEnum), CurrencyEnum.EUR);
            if (dto.WinningAmount > 0)
            {
                result.Add(Create(BetTransactionTypeEnum.Win, (decimal)dto.WinningAmount / 100, eur));
            }

            if (dto.RefundableAmount > 0)
            {
                result.Add(Create(BetTransactionTypeEnum.RefundStake, (decimal)dto.RefundableAmount / 100, eur));
            }

            if (dto.TaxStake > 0)
            {
                result.Add(Create(BetTransactionTypeEnum.TaxStake, (decimal)dto.TaxStake / 100, eur));
            }

            if (dto.TaxWin > 0)
            {
                result.Add(Create(BetTransactionTypeEnum.TaxWin, (decimal)dto.TaxWin / 100, eur));
            }

            return result;
        }
        #endregion

    }
}
