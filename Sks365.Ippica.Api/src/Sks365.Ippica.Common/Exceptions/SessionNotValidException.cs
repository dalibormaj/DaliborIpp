using Sks365.Ippica.Common.Utility;
using System.Net;

namespace Sks365.Ippica.Common.Exceptions
{
    public class SessionNotValidException : BaseException
    {
        public SessionNotValidException(LanguageEnum language = LanguageEnum.English) : base(ReturnCodeEnum.SessionNotValid, ReturnCodeTranslator.Translate(ReturnCodeEnum.SessionNotValid, language))
        {
            StatusCode = HttpStatusCode.OK;
        }

        public SessionNotValidException(string message) : base(ReturnCodeEnum.SessionNotValid, message)
        {
            StatusCode = HttpStatusCode.OK;
        }
    }
}
