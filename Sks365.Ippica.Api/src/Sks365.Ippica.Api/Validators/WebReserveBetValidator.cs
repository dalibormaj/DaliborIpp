using FluentValidation;
using Sks365.Ippica.Api.Dto;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Application.Services.Abstraction;
using Sks365.Ippica.Application.Utility.Authorization;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.SessionTracker.Client;
using System;

namespace Sks365.Ippica.Api.Validators
{
    public class WebReserveBetValidator : AbstractValidator<WebReserveBetRequest<BetDto>>
    {
        public WebReserveBetValidator(ICustomAuthorization customAuthorization, ISessionTracker sessionTracker, IUserService userService)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            //authorization
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
                    .AllowFor(UserStatusEnum.Enabled, UserStatusEnum.PartiallyOpen)
                    .AllowFor(() =>
                    {
                        //it has to be done like this mostly because many users do not have a row in DB for self-exclusion
                        //thus, we cannot use AllowFor(UserDataTypeEnum.SelfExcluded, "0")
                        var userAdditionalData = userService.GetUserAdditionalDataValue((int)user.UserId, UserDataTypeEnum.SelfExcluded);
                        var selfExcluded = string.Equals(userAdditionalData?.Value, "1");
                        return !selfExcluded;
                    })
                    .BuildCondition();

                customAuthorization.ApplyConditions(conditionBuilder.GetResults()).Autorize(user);

                conditionBuilder
                    .HasPermission(ProviderEnum.MstIppica, SectionTypeEnum.IPPICA)
                    .AllowFor(UserTypeEnum.User)
                    .AllowFor(UserStatusEnum.Enabled, UserStatusEnum.PartiallyOpen)
                    .BuildCondition();

                customAuthorization.ApplyConditions(conditionBuilder.GetResults()).Autorize(user);

                return true;
            });

            //Mandatory fields
            RuleFor(x => x.Session).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Session is missing");
            RuleFor(x => x.TicketId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TicketId is missing");
            RuleFor(x => x.Game).NotEmpty().Must(x => string.Equals(x, "QF", StringComparison.OrdinalIgnoreCase)).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Game is missing or contains an invalid value");
            RuleFor(x => x.Currency).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Currency is missing");
            RuleFor(x => x.Amount).NotNull().GreaterThan(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Amount is missing");
            RuleFor(x => x.Bet).NotNull().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Bet is missing");
            RuleFor(x => x.Bet.Header).NotNull().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Bet->Header is missing")
                                      .NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Bet->Header is missing");
            When(x => x.Bet.Header != null, () =>
            {
                RuleFor(x => x.Bet.Header.TicketId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TicketId is missing");
                RuleFor(x => x.Bet.Header.MaxWinning).NotNull().GreaterThan(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("MaxWinning is missing");
            });
            RuleFor(x => x.Bet.Details).NotNull().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Bet->Details is missing");

            //other validations
            RuleFor(x => x).Must(x =>
            {
                return string.Equals(x.TicketId, x.Bet?.Header?.TicketId);
            }).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TicketId has different values")
            .Must(x => x.Amount == x.Bet.Header.Stake + x.TaxStake).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Amount is not equal to Stake + TaxStake");
        }
    }
}