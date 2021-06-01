using Sks365.Ippica.Common.Utility;
using System.Net;

namespace Sks365.Ippica.Common.Exceptions
{
    public class PaymentException : BaseException
    {
        public PaymentException(LanguageEnum language = LanguageEnum.English) : base(ReturnCodeEnum.BadRequest, ReturnCodeTranslator.Translate(ReturnCodeEnum.BadRequest, language))
        {
            StatusCode = HttpStatusCode.OK;
        }

        public PaymentException(ReturnCodeEnum? returnCode, LanguageEnum language = LanguageEnum.English) : base(returnCode, ReturnCodeTranslator.Translate(returnCode, language))
        {
            StatusCode = HttpStatusCode.OK;
        }

        public PaymentException(ReturnCodeEnum? returnCode, string message) : base(returnCode, message)
        {
            StatusCode = HttpStatusCode.OK;
        }
    }
}
