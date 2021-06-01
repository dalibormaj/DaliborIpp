using FluentValidation;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Common.Utility;

namespace Sks365.Ippica.Api.Validators
{
    public class ShopIdentificationValidator : AbstractValidator<ShopIdentificationRequest>
    {
        public ShopIdentificationValidator()
        {
            //Mandatory fields
            RuleFor(x => x.ShopId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("ShopId is missing");
            RuleFor(x => x.TerminalId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TerminalId is missing");
            RuleFor(x => x.Session).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Session is missing");
            RuleFor(x => x.OperatorId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("OperatorId is missing");
        }
    }
}
