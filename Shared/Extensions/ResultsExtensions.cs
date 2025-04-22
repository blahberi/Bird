namespace Shared.Extensions;

public static class ResultsExtensions
{
    public static Result<T> ToResult<T>(this T value)
    {
        return Result<T>.SuccessResult(value);
    }

    public static Result<T> ToErrorAs<T>(this Result<T> result, string defaultError)
    {
        if (result.Error != null)
        {
            return Result<T>.FailureResult(result.Error);
        }

        return Result<T>.FailureResult(defaultError);
    }
}