using FluentValidation;
using Sks365.Ippica.Api.Dto;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Common.Utility;
using System;

namespace Sks365.Ippica.Api.Validators
{
    public class WebPlaceBetValidator : AbstractValidator<WebPlaceBetRequest<BetDto>>
    {
        public WebPlaceBetValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            //Mandatory fields
            RuleFor(x => x.Session).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Session is missing");
            RuleFor(x => x.TicketId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TicketId is missing");
            RuleFor(x => x.ExternalId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("ExternalId is missing");
            RuleFor(x => x.Game).NotEmpty().Must(x => string.Equals(x, "QF", StringComparison.OrdinalIgnoreCase)).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Game is missing or contains an invalid value");
            RuleFor(x => x.Currency).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Currency is missing");
            RuleFor(x => x.Amount).NotNull().GreaterThan(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Amount is missing");
            RuleFor(x => x.Bet).NotNull().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Bet is missing");
            RuleFor(x => x.Bet.Header.TicketId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TicketId is missing");
            RuleFor(x => x.Bet.Header.MaxWinning).NotNull().GreaterThan(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("MaxWinning is missing");
            RuleFor(x => x.Bet.Header.Bets).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Bets missing");
            RuleFor(x => x.Bet.Details).NotNull().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Details missing");
            RuleForEach(x => x.Bet.Details).NotNull().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Details missing");

            //other validations
            RuleFor(x => x).Must(x =>
            {
                return string.Equals(x.TicketId, x.Bet?.Header?.TicketId);
            }).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TicketId has different values")
            .Must(x => x.Amount == x.Bet.Header.Stake + x.TaxStake).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Amount is not equal to Stake + TaxStake");
        }
    }
}