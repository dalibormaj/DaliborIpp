using FluentValidation;
using Sks365.Ippica.Api.Dto;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Application.Utility;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model.Enums;
using System;

namespace Sks365.Ippica.Api.Validators
{
    public class ShopSettleBetValidator : AbstractValidator<ShopSettleBetRequest>
    {
        public ShopSettleBetValidator()
        {
            RuleFor(x => x.Tickets).NotNull().WithState(x => ReturnCodeEnum.BadRequest).When(x => x != null).WithMessage("Tickets is missing");
            RuleForEach(x => x.Tickets).SetValidator(new TicketValidator());
        }

        class TicketValidator : AbstractValidator<TicketDto>
        {
            public TicketValidator()
            {
                //Mandatory fields
                RuleFor(x => x.ShopId).NotNull().GreaterThanOrEqualTo(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("ShopId is missing or it contains an invalid value");
                RuleFor(x => x.WinAmount).NotNull().GreaterThanOrEqualTo(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("WinAmount is missing or it contains an invalid value");
                RuleFor(x => x.RefundAmount).NotNull().GreaterThanOrEqualTo(0).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("RefundAmount is missing or it contains an invalid value");
                RuleFor(x => x.TicketId).NotNull().NotEmpty().WithState(x => ReturnCodeEnum.BadRequest).WithMessage("TicketId is missing");
                RuleFor(x => x.Game).NotEmpty().Must(x =>
                {
                    return GameTypeConverter.GameToBetTypeEnum(x) != null;
                }).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Game is missing or it contains invalid value");
                RuleFor(x => x.Status).NotNull()
                                      .NotEmpty()
                                      .Must(x => Enum.IsDefined(typeof(BetSettlementStatusEnum), x)).WithState(x => ReturnCodeEnum.BadRequest).WithMessage("Status is missing or it contains an invalid value");
                //other validations
                RuleFor(x => x).Must(x =>
                {
                    if (string.Equals(x.Status, "W", StringComparison.OrdinalIgnoreCase))
                    {
                        if (x.WinAmount == 0) throw new IppicaException(ReturnCodeEnum.BadRequest, $"WinAmount is missing or it contains invalid value (TicketId: {x.TicketId})");
                    }
                    else if (string.Equals(x.Status, "L", StringComparison.OrdinalIgnoreCase))
                    {
                        if (x.WinAmount != 0) throw new IppicaException(ReturnCodeEnum.BadRequest, $"WinAmount should be empty for the lost tickets (TicketId: {x.TicketId})");
                        if (x.RefundAmount != 0) throw new IppicaException(ReturnCodeEnum.BadRequest, $"RefundAmount should be empty for the lost tickets (TicketId: {x.TicketId})");
                    }
                    else if (string.Equals(x.Status, "V", StringComparison.OrdinalIgnoreCase))
                    {
                        if (x.WinAmount != 0) throw new IppicaException(ReturnCodeEnum.BadRequest, $"WinAmount should be empty for the refundable tickets (TicketId: {x.TicketId})");
                        if (x.RefundAmount == 0) throw new IppicaException(ReturnCodeEnum.BadRequest, $"RefundAmount is missing or it contains invalid value (TicketId: {x.TicketId})");
                    }
                    return true;
                });

            }
        }
    }
}
