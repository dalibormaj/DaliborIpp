using FluentValidation;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Application.Utility;
using Sks365.Ippica.Common.Utility;

namespace Sks365.Ippica.Api.Validators
{
    public class ShopPayBetValidator : AbstractValidator<ShopPayBetRequest>
    {
        public ShopPayBetValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            //Mandatory fields
            RuleFor(x => x.TicketId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TicketId is missing");
            RuleFor(x => x.Currency).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Currency is missing");
            RuleFor(x => x.Game).NotEmpty().Must(x =>
            {
                return GameTypeConverter.GameToBetTypeEnum(x) != null;
            }).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Game is missing or it contains invalid value");

            //other validations
            RuleFor(x => x.WinningAmount).GreaterThanOrEqualTo(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("WinningAmount cannot be negative");
            RuleFor(x => x.RefundableAmount).GreaterThanOrEqualTo(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("RefundableAmount cannot be negative");
            RuleFor(x => x).Must(x =>
            {
                return x.WinningAmount + x.RefundableAmount != 0;
            }).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Amounts cannot be empty");

        }
    }
}
