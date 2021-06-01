using FluentValidation.Results;

namespace Sks365.Ippica.Common.Utility
{
    public interface IGlobalValidator
    {
        ValidationResult Validate<T>(T instance);
    }
}
