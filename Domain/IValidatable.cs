using WayForPaySDK.Exceptions;

namespace WayForPaySDK.Domain;

public interface IValidatable
{
    IReadOnlyList<string> Validate();
}

internal static class ValidationHelper
{
    internal static void ValidateAndThrow(IValidatable validatable)
    {
        var errors = validatable.Validate();
        if (errors.Count > 0)
        {
            throw new ValidationException($"{validatable.GetType().Name} validation failed.", errors);
        }
    }
}
