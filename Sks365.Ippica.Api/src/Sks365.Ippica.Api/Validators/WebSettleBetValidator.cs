using FluentValidation;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.Api.Validators
{
    public class WebSettleBetValidator : AbstractValidator<WebSettleBetRequest>
    {
        public WebSettleBetValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            //Mandatory fields
            RuleFor(x => x.UserAccount).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("UserAccount is missing")
                                       .Matches(@"^[\d]+$").WithState(x => ReturnCodeEnum.BadRequest).WithMessage("UserAccount must be in numerical format");
            RuleFor(x => x.TicketId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TicketId is missing");
            RuleFor(x => x.Id).NotNull().GreaterThan(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Id is missing");
            RuleFor(x => x.Currency).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Currency is missing");
            RuleFor(x => x.Reason).NotNull().IsInEnum().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Reason is missing");
            RuleFor(x => x.PaymentAmount).GreaterThanOrEqualTo(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("PaymentAmount cannot be negative");
            RuleFor(x => x.TaxWin).GreaterThanOrEqualTo(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TaxWin cannot be negative");
            RuleFor(x => x.RefundAmount).GreaterThanOrEqualTo(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("RefundAmount cannot be negative");
            RuleFor(x => x.TaxStake).GreaterThanOrEqualTo(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TaxStake cannot be negative");


            //other validations
            RuleFor(x => x).Must(x =>
            {
                if (x.Reason == BetSettlementReasonEnum.Payment) // Win
                {
                    if (x.RefundAmount > 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "Choose a different reason (7) if you want to compensate stake");

                    if (x.PaymentAmount <= 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "PaymentAmount is missing or contains invalid value");
                }
                if (x.Reason == BetSettlementReasonEnum.Refund)
                {
                    if (x.PaymentAmount > 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "PaymentAmount contains invalid value. Choose a different reason (1) if you want to settle win");

                    if (x.TaxWin > 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "TaxWin contains invalid value");
                }

                else if (x.Reason == BetSettlementReasonEnum.Losers)
                {
                    if (x.PaymentAmount != 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "Field PaymentAmount should be empty for the lost tickets");
                    if (x.RefundAmount != 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "Field RefundAmount should be empty for the lost tickets");
                    if (x.TaxWin != 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "Field TaxWin should be empty for the lost tickets");
                }
                else if (x.Reason == BetSettlementReasonEnum.CancelPayment)
                {
                    if (x.RefundAmount != 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "Field RefundAmount should be empty for the reason 3 (CancelPayment)");
                }
                else if (x.Reason == BetSettlementReasonEnum.CancelRefund)
                {

                    if (x.PaymentAmount != 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "Field PaymentAmount should be empty for the reason 4 (CancelRefund)");

                    if (x.TaxWin != 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "Field TaxWin should be empty for the reason 4 (CancelRefund)");
                }
                else if (x.Reason == BetSettlementReasonEnum.TicketReopened)
                {
                    if (x.RefundAmount != 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "Field RefundAmount should be empty for the reason 6 (TicketReopened)");
                    if (x.PaymentAmount != 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "Field PaymentAmount should be empty for the reason 6 (TicketReopened)");
                    if (x.TaxWin != 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "Field TaxWin should be empty for the reason 6 (TicketReopened)");
                }
                else if (x.Reason == BetSettlementReasonEnum.PaymentAndRefund)
                {
                    if (x.PaymentAmount <= 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "PaymentAmount is missing or contains invalid value");

                    if (x.RefundAmount <= 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "RefundAmount is missing or contains invalid value");
                }
                else if (x.Reason == BetSettlementReasonEnum.CancelPaymentAndRefund)
                {
                    if (x.PaymentAmount <= 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "PaymentAmount is missing or contains invalid value");

                    if (x.RefundAmount <= 0)
                        throw new IppicaException(ReturnCodeEnum.BadRequest, "RefundAmount is missing or contains invalid value");
                }

                if (x.TotalAmount != x.PaymentAmount + x.RefundAmount)
                    throw new IppicaException(ReturnCodeEnum.BadRequest, "TotalAmount is not equal to PaymentAmount + RefundAmount");

                return true;
            });
        }
    }
}