using Sks365.Ippica.Common.Utility;
using System.Net;

namespace Sks365.Ippica.Common.Exceptions
{
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(LanguageEnum language = LanguageEnum.English) : base(ReturnCodeEnum.Forbidden, ReturnCodeTranslator.Translate(ReturnCodeEnum.Forbidden, language))
        {
            StatusCode = HttpStatusCode.OK;
        }

        public ForbiddenException(string message) : base(ReturnCodeEnum.Forbidden, message)
        {
            StatusCode = HttpStatusCode.OK;
        }
    }
}
