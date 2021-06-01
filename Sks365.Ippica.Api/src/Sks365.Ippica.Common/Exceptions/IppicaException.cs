using Sks365.Ippica.Common.Utility;
using System.Net;

namespace Sks365.Ippica.Common.Exceptions
{
    public class IppicaException : BaseException
    {
        public IppicaException(LanguageEnum language = LanguageEnum.English) : base(ReturnCodeEnum.BadRequest, ReturnCodeTranslator.Translate(ReturnCodeEnum.BadRequest, language))
        {
            StatusCode = HttpStatusCode.OK;
        }

        public IppicaException(ReturnCodeEnum? returnCode, LanguageEnum language = LanguageEnum.English) : base(returnCode, ReturnCodeTranslator.Translate(returnCode, language))
        {
            StatusCode = HttpStatusCode.OK;
        }

        public IppicaException(ReturnCodeEnum? returnCode, string message) : base(returnCode, message)
        {
            StatusCode = HttpStatusCode.OK;
        }
    }
}
