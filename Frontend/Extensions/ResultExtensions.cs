using Shared;
using Shared.Extensions;
using Frontend.Core;

namespace Frontend.Extensions;

public static class ResultsExtensions
{
    public static Result<T, Error> OkIf<T>(this T? value, Func<T, bool> predicate, Error error)
    {
        return value.OkIf(predicate, () => error);
    }
    
    public static Result<T, Error> OkIf<T>(this T? value, Func<T, bool> predicate, string errorMessage)
    {
        return value.OkIf(predicate, new Error(
            errorMessage, 
            new Dictionary<string, string>()));
    }
    
    public static Result<T, Error> ErrIf<T>(this T? value, Func<T, bool> predicate, Error error)
    {
        return value.ErrIf(predicate, () => error);
    }
    
    public static Result<T, Error> ErrIf<T>(this T? value, Func<T, bool> predicate, string errorMessage)
    {
        return value.ErrIf(predicate, new Error(
            errorMessage, 
            new Dictionary<string, string>()));
    }
    
    public static Result<T, Error> ToResult<T>(this T? value, Error error)
    {
        return value.ToResult(() => error);
    }
    
    public static Result<T, Error> ToResult<T>(this T? value, string errorMessage)
    {
        return value.ToResult(new Error(
            errorMessage, 
            new Dictionary<string, string>()));
    }
    
    public static Result<T, Error> ToOkResult<T>(this T value)
    {
        return value.ToOkResult<T, Error>();
    }
    
    public static Result<IEnumerable<T>, Error> ToEnumerableResult<T>(this IEnumerable<T> values, Error error)
    {
        return values.ToEnumerableResult(() => error);
    }
    
    public static Result<IEnumerable<T>, Error> ToEnumerableResult<T>(this IEnumerable<T> values, string errorMessage)
    {
        return values.ToEnumerableResult(new Error(
            errorMessage, 
            new Dictionary<string, string>()));
    }
}