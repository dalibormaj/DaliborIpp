using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sks365.Ippica.Api.Dto.Responses;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Payments.WebApi.Client;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Sks365.Ippica.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleGlobalExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleGlobalExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            ErrorResponse response = new ErrorResponse();

            //Cover exceptions fired from ASYNC methods
            if (exception is AggregateException && exception.InnerException != null && exception.InnerException is BaseException)
            {
                exception = exception.InnerException;
            }

            if (exception is BaseException && !(exception is PaymentException))
                _logger.LogWarning("Exception:{@Exception}\nInnerException:{@InnerException}", exception, exception.InnerException);
            else
                _logger.LogError("Exception:{@Exception}\nInnerException:{@InnerException}", exception, exception.InnerException);

            context.Response.StatusCode = (int)GetHttpStatusCode(exception);
            response = GetErrorResponse(exception);

            return context.Response.WriteAsync(response?.ToString());
        }

        private ErrorResponse GetErrorResponse(Exception exception)
        {
            ErrorResponse errorResponse = null;
            if (exception is BaseException)
            {
                var ex = (BaseException)exception;
                errorResponse = new ErrorResponse()
                {
                    ReturnCode = ex.ReturnCode ?? ReturnCodeEnum.Unknown,
                    Description = ex.Message,
                    Timestamp = DateTime.Now.ToMicrosoftDate()
                };
            }
            else if (exception is PaymentsClientException<ErrorInfo> ex)
            {
                var innerError = ex?.Result?.InnerError;
                while (innerError != null)
                {
                    errorResponse = new ErrorResponse()
                    {
                        ReturnCode = ReturnCodeEnum.TransactionNotCreated,
                        Description = ex.Result?.InnerError?.Message,
                        Timestamp = DateTime.Now.ToMicrosoftDate()
                    };
                    innerError = innerError.InnerError;
                }
            }
            else
            {
                errorResponse = new ErrorResponse()
                {
                    ReturnCode = ReturnCodeEnum.Unknown,
                    Description = exception.Message,
                    Timestamp = DateTime.Now.ToMicrosoftDate()
                };
            }

            return errorResponse;
        }


        private HttpStatusCode GetHttpStatusCode(Exception exception)
        {

            if (exception is BaseException ex)
            {
                return ex.StatusCode;
            }
            else if (exception is PaymentsClientException)
            {
                return HttpStatusCode.OK;
            }
            else return HttpStatusCode.InternalServerError;
        }
    }
}
