using FluentValidation;
using Sks365.Ippica.Api.Dto;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Application.Services;
using Sks365.Ippica.Application.Services.Abstraction;
using Sks365.Ippica.Application.Utility.Authorization;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.SessionTracker.Client;
using System;

namespace Sks365.Ippica.Api.Validators
{
    public class ShopReserveBetValidator : AbstractValidator<ShopReserveBetRequest<BetDto>>
    {
        public ShopReserveBetValidator(ICustomAuthorization customAuthorization, ISessionTracker sessionTracker, IUserService userService)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x).Must(x =>
            {

                var sessionData = sessionTracker.GetSession(x.Session).Result;
                if (!sessionData.SessionExists) throw new SessionNotValidException();

                var user = userService.GetUser(sessionData.Username, (BookmakerEnum)sessionData.BookmakerId);

                //Custom authorization
                var conditionBuilder = new CustomAuthorizationConditionsBuilder();

                conditionBuilder
                    .HasPermission(ProviderEnum.MstIppica, SectionTypeEnum.IPPICA)
                    .AllowFor(UserTypeEnum.User)
                    .AllowFor(UserStatusEnum.Enabled)
                    .AllowFor(UserParameterTypeEnum.CdcUser, "1")
                    .BuildCondition();

                customAuthorization.ApplyConditions(conditionBuilder.GetResults()).Autorize(user);

                return true;
            });

            //Mandatory fields
            RuleFor(x => x.OperatorId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("OperatorId is missing");
            RuleFor(x => x.ShopId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("ShopId is missing");
            RuleFor(x => x.TerminalId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TerminalId is missing");
            RuleFor(x => x.Session).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Session is missing");
            RuleFor(x => x.TicketId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TicketId is missing");
            RuleFor(x => x.Game).NotEmpty().Must(x => string.Equals(x, "QF", StringComparison.OrdinalIgnoreCase)).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Game is missing or contains an invalid value");
            RuleFor(x => x.Currency).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Currency is missing");
            RuleFor(x => x.Amount).NotNull().GreaterThan(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Amount is missing or invalid");
            RuleFor(x => x.JBet).NotNull().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("JBet is missing");
            RuleFor(x => x.JBet.Header).NotNull().WithState(x => ReturnCodeEnum.BadRequest).When(x => x.JBet != null).WithMessage("Bet->Header is missing")
                                      .NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).When(x => x.JBet != null).WithMessage("Bet->Header is missing");
            When(x => x.JBet != null && x.JBet.Header != null, () =>
            {
                RuleFor(x => x.JBet.Header.TicketId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TicketId is missing");
                RuleFor(x => x.JBet.Header.MaxWinning).NotNull().GreaterThan(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("MaxWinning is missing");
            });
            RuleFor(x => x.JBet.Details).NotNull().WithState(x => ReturnCodeEnum.BadRequest).When(x => x.JBet != null).WithMessage("JBet->Details is missing");

            //other validations
            RuleFor(x => x).Must(x =>
            {
                return string.Equals(x.TicketId, x.JBet?.Header?.TicketId);
            }).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TicketId has different values")
              .Must(x => x.Amount == x.JBet.Header.Stake + x.TaxStake).WithState(x => ReturnCodeEnum.BadRequest).When(x => x.JBet != null).WithMessage("Amount is not equal to Stake + TaxStake");
        }
    }
}
