using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sks365.Ippica.Common.Exceptions;
using System.Linq;

namespace Sks365.Ippica.Common.Utility
{
    public class GlobalValidator : IGlobalValidator
    {
        private HttpContext httpContext;

        public GlobalValidator(IHttpContextAccessor context)
        {
            httpContext = context.HttpContext;
        }

        public ValidationResult Validate<T>(T instance)
        {
            if (instance == null)
                throw new IppicaException(ReturnCodeEnum.BadRequest, "Cannot validate an empty object");

            var validator = httpContext.RequestServices.GetService<IValidator<T>>();
            if (validator != null)
            {
                var validationResults = validator.Validate(instance);

                //Show the first error message if exists (Default IppicaException) 
                if ((validationResults.Errors?.Count ?? 0) > 0)
                {
                    var error = validationResults.Errors.FirstOrDefault();
                    var returnCode = (error?.CustomState is ReturnCodeEnum) ? (ReturnCodeEnum)error?.CustomState : ReturnCodeEnum.BadRequest;

                    throw new IppicaException(returnCode, error.ErrorMessage);
                }

                return validationResults;
            }

            return null;
        }
    }
}
