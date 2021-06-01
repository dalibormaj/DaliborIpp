using FluentValidation;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Common.Utility;

namespace Sks365.Ippica.Api.Validators
{
    public class WebRollbackBetValidator : AbstractValidator<WebRollbackBetRequest>
    {
        public WebRollbackBetValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            //Mandatory fields
            RuleFor(x => x.Session).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Session is missing");
            RuleFor(x => x.TicketId).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TicketId is missing");
            RuleFor(x => x.Ip).NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Ip is missing");
        }
    }
}