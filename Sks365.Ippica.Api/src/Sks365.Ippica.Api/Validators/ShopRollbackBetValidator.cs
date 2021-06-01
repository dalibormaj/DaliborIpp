using FluentValidation;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Common.Utility;
using System;
using System.Linq;

namespace Sks365.Ippica.Api.Validators
{
    public class ShopRollbackBetValidator : AbstractValidator<ShopRollbackBetRequest>
    {
        public ShopRollbackBetValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            var possibleGames = new[] { "QF", "PSR", "TOT" };

            //Mandatory fields
            RuleFor(x => x.ShopId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("ShopId is missing");
            RuleFor(x => x.TicketId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TicketId is missing");
            RuleFor(x => x.Game).NotEmpty().Must(x => possibleGames.Contains(x, StringComparer.OrdinalIgnoreCase)).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Game is missing or contains an invalid value");
            RuleFor(x => x.Session).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Session is missing");
        }
    }
}
