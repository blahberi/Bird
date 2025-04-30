using Backend.Core;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTOs;
using Shared.Extensions;

namespace Backend.Extensions;

public static class ResultsExtensions
{
    public static IActionResult ToActionResult<T, E>(
        this Result<T, E> result,
        Func<T, IActionResult> onSuccess,
        Func<E, IActionResult> onError)
    {
        return result.Match(onSuccess, onError);
    }
    
    public static IActionResult ToActionResult<T>(this Result<T, Error> result)
    {
        return result.ToActionResult(
            onSuccess: value => new OkObjectResult(value),
            onError: error =>
            {
                return error.Type switch
                {
                    ErrorType.Unauthorized => new UnauthorizedObjectResult(new ErrorResponse
                    {
                        Error = error.Message, Details = error.Details
                    }),
                    _ => new BadRequestObjectResult(new ErrorResponse
                    {
                        Error = error.Message, Details = error.Details
                    })
                };
            }
        );
    }
    
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