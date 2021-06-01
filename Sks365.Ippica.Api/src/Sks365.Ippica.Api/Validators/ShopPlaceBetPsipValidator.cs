using FluentValidation;
using Sks365.Ippica.Api.Dto;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Common.Utility;
using System;

namespace Sks365.Ippica.Api.Validators
{
    public class ShopPlaceBetPsipValidator : AbstractValidator<ShopPlaceBetRequest<PsipBetDto>>
    {
        public ShopPlaceBetPsipValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            //Mandatory fields
            RuleFor(x => x.Session).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Session is missing");
            RuleFor(x => x.TicketId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TicketId is missing");
            RuleFor(x => x.ExternalId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("ExternalId is missing");
            RuleFor(x => x.Game).NotEmpty().Must(x => string.Equals(x, "TOT", StringComparison.OrdinalIgnoreCase)).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Game is missing or contains an invalid value");
            RuleFor(x => x.Currency).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Currency is missing");
            RuleFor(x => x.Amount).NotNull().GreaterThan(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Amount is missing or invalid");
            RuleFor(x => x.JBet).NotNull().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("JBet is missing");
            RuleFor(x => x.JBet.Prezzo).NotNull().GreaterThan(0).WithState(x => ReturnCodeEnum.BadRequest).When(x => x.JBet != null).WithMessage("Prezzo is missing or invalid");
            RuleFor(x => x.JBet.Scommessa).NotNull().WithState(x => ReturnCodeEnum.BadRequest).When(x => x.JBet != null).WithMessage("Scommessa is missing")
                                          .NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).When(x => x.JBet != null).WithMessage("Scommessa is missing");

            //other validations
            RuleFor(x => x).Must(x =>
            {
                return string.Equals(x.Session, x.JBet?.Session);
            }).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Session has different values")
            .Must(x => x.Amount == x.JBet.Prezzo + x.TaxStake).WithState(x => ReturnCodeEnum.BadRequest).When(x => x.JBet != null).WithMessage("Amount is not equal to Prezzo + TaxStake");
        }
    }
}
