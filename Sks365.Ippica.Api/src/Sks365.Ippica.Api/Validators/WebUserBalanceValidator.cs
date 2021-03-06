using FluentValidation;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Common.Utility;

namespace Sks365.Ippica.Api.Validators
{
    public class WebUserBalanceValidator : AbstractValidator<WebUserBalanceRequest>
    {
        public WebUserBalanceValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            //Mandatory fields
            RuleFor(x => x.Session).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Session is missing");
        }
    }
}
