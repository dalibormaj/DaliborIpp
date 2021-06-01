using FluentValidation;
using Sks365.Ippica.Api.Dto.Requests;

namespace Sks365.Ippica.Api.Validators
{
    public class ShopKeepAliveValidator : AbstractValidator<ShopKeepAliveRequest>
    {
        public ShopKeepAliveValidator()
        {
        }
    }
}
