using Sks365.Ippica.Common.Utility;
using System;
using System.Net;

namespace Sks365.Ippica.Common.Exceptions
{
    public abstract class BaseException : Exception
    {
        public ReturnCodeEnum? ReturnCode { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public BaseException(ReturnCodeEnum? returnCode, string message) : base(message)
        {
            ReturnCode = returnCode;
        }
    }
}
